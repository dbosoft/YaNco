

using Dbosoft.YaNco;
using Microsoft.Extensions.Configuration;
using RfcServerTest;

var configurationBuilder =
    new ConfigurationBuilder();

configurationBuilder.AddInMemoryCollection(new[]
{
    new KeyValuePair<string, string>("tests:repeats", "10"),
    new KeyValuePair<string, string>("tests:rows", "10")
});
configurationBuilder.AddEnvironmentVariables("saprfc");
configurationBuilder.AddCommandLine(args);
configurationBuilder.AddUserSecrets<Program>();

var config = configurationBuilder.Build();

var serverSettings = new Dictionary<string, string>
{
    {"SYSID", "NA1"},
    {"PROGRAM_ID", "YANCO_TEST"},
    {"GWHOST",config["saprfc:ashost"]},
    {"GWSERV", "sapgw" + config["saprfc:sysnr"]},
    {"REG_COUNT", "2"},
    {"TRACE", "1"}

};

var clientSettings = new Dictionary<string, string>
{
    {"ashost", config["saprfc:ashost"]},
    {"sysnr", config["saprfc:sysnr"]},
    {"client", config["saprfc:client"]},
    {"user", config["saprfc:username"]},
    {"passwd", config["saprfc:password"]},
    {"lang", "EN"},
};


var cancellationTokenSource = new CancellationTokenSource();


var serverBuilderWithClientConnection = new ServerBuilder(serverSettings)
    .ConfigureRuntime(c =>
        c.WithLogger(new SimpleConsoleLogger()))

    .WithClientConnection(clientSettings, 
        c => c
            .WithFunctionHandler("ZYANCO_SERVER_FUNCTION_1",
            cf => cf
                .Input(i => i.GetField<string>("SEND"))
                .ProcessAsync(s =>
                {
                    Console.WriteLine($"Received message from backend: {s}");
                    cancellationTokenSource.Cancel();

                    return cf.UseRfcContext(context =>
                    {
                        return from connection in context.GetConnection()
                            from attributes in connection.GetAttributes()
                            from userName in context.CallFunction("BAPI_USER_GET_DETAIL",
                                Input: f => f.SetField("USERNAME", attributes.User),
                                Output: f => f.MapStructure("ADDRESS", s => s.GetField<string>("FULLNAME")))
                            select userName;

                    }).Match(r => r, l => "John Doe");

                })
                .Reply((username, f) => f
                    .SetField("RECEIVE", $"Hello {username}!")))
        );


var rfcServer = await serverBuilderWithClientConnection.Build()
    .StartOrException();

Console.WriteLine("Server with client connection is ready");
Console.WriteLine("call function ZYANCO_SERVER_FUNCTION_1 from backend");

try
{
    await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
}
catch(TaskCanceledException){}


await rfcServer.Stop().ToEither();

Console.WriteLine("Server stopped");


cancellationTokenSource = new CancellationTokenSource();

var serverBuilderWithoutClientConnection = new ServerBuilder(serverSettings)
    .WithFunctionHandler(
        "ZYANCO_SERVER_FUNCTION_2",
        b => b
            .AddChar("SEND", RfcDirection.Import, 30)
            .AddChar("RECEIVE", RfcDirection.Export, 30),
        cf => cf
            .Input(i =>
                i.GetField<string>("SEND"))
            .Process(s =>
            {
                Console.WriteLine($"Received message from backend: {s}");
                cancellationTokenSource.Cancel();
                
            })
            .Reply((_, f) => f
                .SetField("RECEIVE", "Hello from YaNco")));

rfcServer = await serverBuilderWithoutClientConnection.Build()
    .StartOrException();

Console.WriteLine("Server without client connection is ready");
Console.WriteLine("call function ZYANCO_SERVER_FUNCTION_2 from backend");
Console.WriteLine("Server ready");

try
{
    await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
}
catch (TaskCanceledException) { }


await rfcServer.Stop().ToEither();

Console.WriteLine("Server stopped");
