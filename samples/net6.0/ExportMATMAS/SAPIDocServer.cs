using Dbosoft.YaNco;
using Dbosoft.YaNco.Live;
using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
// ReSharper disable StringLiteralTypo

namespace ExportMATMAS;

// ReSharper disable InconsistentNaming
public enum TransactionState
{
    Created,
    Executed,
    Committed,
    RolledBack

}
// ReSharper restore InconsistentNaming

public class SAPIDocServer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly TransactionManager<MaterialMasterRecord> _transactionManager = new();

    public SAPIDocServer(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {


        var serverSettings = new Dictionary<string, string>
        {

            { "SYSID", _configuration["saprfc:sysid"] },
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


        using var rfcServer = await new ServerBuilder(serverSettings)
            .WithTransactionalRfc(new MaterialMasterTransactionalRfcHandler<SAPRfcRuntime>(_transactionManager))
            .WithClientConnection(clientSettings,
                c => c
                    .WithFunctionHandler("IDOC_INBOUND_ASYNCHRONOUS", ProcessInboundIDoc))
            .Build()
            .StartOrException();


        Console.WriteLine("MATMAS IDOC Server is ready");

        try
        {
            await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
        }
        catch (TaskCanceledException)
        {
        }


        _ = await rfcServer.Stop().ToEither();

        Console.WriteLine("Server stopped");
    }


    private Aff<SAPRfcRuntime, Unit> ProcessInboundIDoc(CalledFunction<SAPRfcRuntime> cf) =>
        SAPIDocServer<SAPRfcRuntime>.processInboundIDoc(cf, _transactionManager);
}


// ReSharper disable InconsistentNaming
// ReSharper restore InconsistentNaming