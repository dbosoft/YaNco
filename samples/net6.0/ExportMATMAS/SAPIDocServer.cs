using System.Text.Json;
using Dbosoft.YaNco;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SAPSystemTests;

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
            { "TRACE", "5" }
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
                return RfcErrorInfo.Ok();
            }, onCommit: (handle, tid) =>
            {
                return RfcErrorInfo.Ok();
            },
            onRollback: (handle, tid) =>
            {
                return RfcErrorInfo.Ok();
            },
            onConfirm: (handle, tid) =>
            {
                return RfcErrorInfo.Ok();
            }

        );
        
        var serverBuilderWithClientConnection = new ServerBuilder(serverSettings)
            .ConfigureRuntime(c =>
                c.WithLogger(new SimpleConsoleLogger()))
            .WithClientConnection(clientSettings,
                c => c
                    .WithFunctionHandler("IDOC_INBOUND_ASYNCHRONOUS",
                        cf => cf
                            .Input(i =>
                                from control in i.MapTable("IDOC_CONTROL_REC_40",
                                    s => s.ToDictionary())
                                from data in i.MapTable("IDOC_DATA_REC_40",
                                    s => s.ToDictionary())
                                select new
                                {
                                    ControlRecord = control,
                                    DataRecord = data
                                })
                            .ProcessAsync(s =>
                            {
                                return cf.UseRfcContext(context =>
                                {
                                    return context.GetConnection().Map(connection =>
                                    {
                                        var jsonOptions = new JsonSerializerOptions();
                                        jsonOptions.Converters.Add(
                                            new AbapValueJsonConverter(connection.RfcRuntime.FieldMapper));
                                        var idocControlJson = JsonSerializer.Serialize(s.DataRecord, jsonOptions);
                                        Console.WriteLine(idocControlJson);
                                        return Task.FromResult(Unit.Default);
                                    }).IfLeft(l =>
                                    {
                                        Console.WriteLine(l.Message);
                                    });

                                });

                            })
                            .NoReply()
                    ));


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
}