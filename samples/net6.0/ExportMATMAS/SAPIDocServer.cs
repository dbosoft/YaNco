using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using Dbosoft.YaNco;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ExportMATMAS;

public class SAPIDocServer : BackgroundService
{
    private readonly IConfiguration _configuration;
 

    public SAPIDocServer(IConfiguration configuration)
    {
        _configuration = configuration; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serverSettings = new Dictionary<string, string>
        {
            { "SYSID", "saprfc:sysid" },
            { "PROGRAM_ID", _configuration["saprfc:program_id"] },
            { "GWHOST", _configuration["saprfc:ashost"] },
            { "GWSERV", "sapgw" + _configuration["saprfc:sysnr"] },
            { "REG_COUNT", "1" },
            { "TRACE", "0" }
        };

        var clientSettings = new Dictionary<string, string>
        {
            { "ashost", _configuration["saprfc:ashost"] },
            { "sysnr", _configuration["saprfc:sysnr"] },
            { "client", _configuration["saprfc:client"] },
            { "user", _configuration["saprfc:user"] },
            { "passwd", _configuration["saprfc:passwd"] },
            { "lang", "EN" }
        };


        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

        var rfcRuntime = new RfcRuntime(new SimpleConsoleLogger());
        rfcRuntime.AddTransactionHandlers("ID8", onCheck: (handle, tid) =>
            {
                return RfcRc.RFC_OK;
            }, onCommit: (handle, tid) =>
            {
                return RfcRc.RFC_OK;
            },
            onRollback: (handle, tid) =>
            {
                return RfcRc.RFC_OK;
            },
            onConfirm: (handle, tid) =>
            {
                return RfcRc.RFC_OK;
            }

        );

        var serverBuilderWithClientConnection = new ServerBuilder(serverSettings)
            .ConfigureRuntime(c =>
                c.WithLogger(new SimpleConsoleLogger()))
            .WithClientConnection(clientSettings,
                c => c
                    .WithFunctionHandler("IDOC_INBOUND_ASYNCHRONOUS",ProcessInboundIDoc));


        var rfcServer = await serverBuilderWithClientConnection.Build()
            .StartOrException();

        Console.WriteLine("MATMAS IDOC Server is ready");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
        }


        await rfcServer.Stop().ToEither();

        Console.WriteLine("Server stopped");
    }


    private static EitherAsync<RfcErrorInfo, Unit> ProcessInboundIDoc(CalledFunction cf)
    {
        return cf
            .Input(i =>
                from data in i.MapTable("IDOC_DATA_REC_40",
                    s => 
                        from iDocNo in s.GetField<string>("DOCNUM")
                        from segment in s.GetField<string>("SEGNAM")
                        from segmentNo in s.GetField<int>("SEGNUM")
                        from parentNo in s.GetField<int>("PSGNUM")
                        from level in s.GetField<int>("HLEVEL")
                        from data in s.GetField<string>("SDATA")
                        select new IDocDataRecord(iDocNo, segment, segmentNo, parentNo, level, data)
                        )
                select data.ToSeq())
            .ProcessAsync(data =>
            {
                cf.RfcRuntime.GetServerContext(cf.RfcHandle).Map(context =>
                {
                    Console.WriteLine("current tid: " + context.TransactionId);
                    return context;
                });

                return cf.UseRfcContextAsync(context =>
                {
                    return (from connection in context.GetConnection()
                        from materialMaster in ExtractMaterialMaster(connection, data)
                        select materialMaster).Match(
                        r => Console.WriteLine("Received Material:\n" + PrettyPrintMaterial(r)),
                        l => Console.WriteLine("Error: " + l.Message)); ;
                });


            })
            .NoReply();
    }


    private static EitherAsync<RfcErrorInfo, MaterialMasterRecord> ExtractMaterialMaster(IConnection connection, 
        Seq<IDocDataRecord> data)
    {
        return
            from clientSegment in FindRequiredSegment("E1MARAM", data)
            from material in MapSegment(connection, clientSegment, s =>
                from materialNo in s.GetField<string>("MATNR")
                from unit in s.GetField<string>("MEINS")
                select new
                {
                    MaterialNo = materialNo, 
                    ClientData = new ClientData(unit)
                })

            from descriptionData in MapSegments(connection, FindSegments("E1MAKTM", data), s =>
                from language in s.GetField<string>("SPRAS_ISO")
                from description in s.GetField<string>("MAKTX")
                select new DescriptionData(language, description))

            from plantData in MapSegments(connection, FindSegments("E1MARCM", data), s =>
                from plant in s.GetField<string>("WERKS")
                from purchasingGroup in s.GetField<string>("EKGRP")
                select new PlantData(plant, purchasingGroup)
            )
            select new MaterialMasterRecord(
                material.MaterialNo, 
                material.ClientData, 
                descriptionData.ToArray(), 
                plantData.ToArray());
    }

    private static EitherAsync<RfcErrorInfo, T> MapSegment<T>(IConnection connection, 
        IDocDataRecord data, Func<IStructure,Either<RfcErrorInfo, T>> mapFunc)
    {
        return connection.CreateStructure(Segment2Type[data.Segment]).Use(structure =>
        {
            return from _ in structure.Bind(s => s.SetFromString(data.Data).ToAsync())
                from res in structure.Bind(s => mapFunc(s).ToAsync())
                select res;
        });

    }

    private static EitherAsync<RfcErrorInfo, Seq<T>> MapSegments<T>(IConnection connection,
        Seq<IDocDataRecord> data, Func<IStructure, Either<RfcErrorInfo, T>> mapFunc)
    {
        return data.Map(segment => MapSegment(connection, segment, mapFunc))
            .Traverse(l => l);
    }

    private static EitherAsync<RfcErrorInfo, IDocDataRecord> FindRequiredSegment(
        string typeName, Seq<IDocDataRecord> records )
    {
        var segmentName = Type2Segment[typeName];
        return records.Find(x => x.Segment == segmentName)
            .ToEither(RfcErrorInfo.Error($"Segment {segmentName} not found"))
            .ToAsync();
    }

    private static Seq<IDocDataRecord> FindSegments(
        string typeName, Seq<IDocDataRecord> records)
    {
        var segmentName = Type2Segment[typeName];
        return records.Filter(x => x.Segment == segmentName);
    }

    // for a known IDoc type you used fixed segment to type mapping
    // a more generic way would be looking up segment names from RFM IDOCTYPE_READ_COMPLETE

    private static HashMap<string, string> Segment2Type = new(new[]
    {
        ("E2MARAM009", "E1MARAM"), // client data, MATMAS05
        ("E2MARCM008", "E1MARCM"), // plant data, MATMAS05
        ("E2MAKTM001", "E1MAKTM"), // descriptions, MATMAS05
    });

    private static HashMap<string, string> Type2Segment = new(new[]
    {
        ("E1MARAM", "E2MARAM009" ),
        ("E1MARCM", "E2MARCM008"),
        ("E1MAKTM", "E2MAKTM001")
    });

    private static string PrettyPrintMaterial(MaterialMasterRecord record)
    {
        return JsonSerializer.Serialize(record, JsonOptions);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
}

public record IDocDataRecord(string IDocNo, string Segment, int SegmentNo, int ParentNo, int Level, string Data);

public record MaterialMasterRecord(string MaterialNo, ClientData ClientData, DescriptionData[] Descriptions, PlantData[] PlantData);


public record ClientData(string Unit);

public record DescriptionData(string Language, string Description);

public record PlantData(string Plant, string PurchasingGroup);


