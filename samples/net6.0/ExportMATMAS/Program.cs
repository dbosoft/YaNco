using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.Live;
using Dbosoft.YaNco;
using ExportMATMAS;
using Microsoft.Extensions.Configuration;
using LanguageExt;
using ExportMATMAS.MaterialMaster;
using static Dbosoft.YaNco.SAPRfcServer<ExportMATMAS.ConsoleRuntime>;
using static ExportMATMAS.SAPIDocServer<ExportMATMAS.ConsoleRuntime>;
using static LanguageExt.Sys.Console<ExportMATMAS.ConsoleRuntime>;

// ReSharper disable CommentTypo

[assembly: ExcludeFromCodeCoverage]

//This is a sample application that can be used to export SAP Material Master from IDocs with YaNco
//Prerequisites: 
// - a SAP System with Material Master (ERP or S/4)
// - maintained connection settings either in appsettings.json file, or in user secrets
// - a outbound idoc configuration in the SAP system that sends MATMAS to destination YANCO_MATMAS

var configurationBuilder =
    new ConfigurationBuilder();

configurationBuilder.AddJsonFile("appsettings.json", true, false);
configurationBuilder.AddUserSecrets<Program>();

var configuration = configurationBuilder.Build();

// ReSharper disable StringLiteralTypo
var serverSettings = new Dictionary<string, string>
        {

            { "SYSID", configuration["saprfc:sysid"] },
            { "PROGRAM_ID", configuration["saprfc:program_id"] },
            { "GWHOST", configuration["saprfc:ashost"] },
            { "GWSERV", "sapgw" + configuration["saprfc:sysnr"] },
            { "REG_COUNT", "1" },
            { "TRACE", "0" }
        };

var clientSettings = new Dictionary<string, string>
        {
            { "ashost", configuration["saprfc:ashost"] },
            { "sysnr", configuration["saprfc:sysnr"] },
            { "client", configuration["saprfc:client"] },
            { "user", configuration["saprfc:user"] },
            { "passwd", configuration["saprfc:passwd"] },
            { "lang", "EN" }
        };
// ReSharper restore StringLiteralTypo

// create the runtime that will be used to run the server
var runtime = ConsoleRuntime.New(new CancellationTokenSource(),
    new SAPServerSettings(null, SAPRfcRuntime.Default.Env.Settings.FieldMapper,
        new RfcRuntimeOptions(), new TransactionManager<MaterialMasterRecord>()));

// build the server IO effect
var serverIO = 
        from serverAff in buildServer(serverSettings,

            // IDocs have to be processed with a transactional RFC handler
        c => c.WithTransactionalRfc(
                new MaterialMasterTransactionalRfcHandler<ConsoleRuntime>())
        .WithClientConnection(clientSettings,
            cc => cc
                .WithFunctionHandler("IDOC_INBOUND_ASYNCHRONOUS", processInboundIDoc)))
               from _ in useServer(serverAff, rfcServer =>

                       from uInfo in writeLine("MATMAS IDOC Server is ready")
                       from uStop in writeLine("Press any key to stop the server")
                       from uStopped in readKey
                       from _ in stopServer(rfcServer)
                       select uInfo
                   )
               from uStopped in writeLine("Server stopped")
               select _;

// run the IO effect to creates and stops the server with the runtime
var res = await serverIO.Run(runtime);

res.IfFail(ex => Console.WriteLine(ex.ToString()));