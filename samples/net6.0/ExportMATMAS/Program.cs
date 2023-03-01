
using System.Diagnostics.CodeAnalysis;
using ExportMATMAS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YaNco.Hosting;

[assembly: ExcludeFromCodeCoverage]

//This is a sample application that can be used to export SAP Material Master from IDocs with YaNco
//Prerequisites: 
// - a SAP System with Material Master (ERP or S/4)
// - maintained connection settings either in appsettings.json file, or in user secrets
// - a outbound idoc configuration in the SAP system that sends MATMAS to destination YANCO_MATMAS

var host = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration(cfg =>
        cfg.AddEnvironmentVariables("saprfc"))
    .ConfigureServices(services => services
        .AddYaNco()
        .AddHostedService<SAPIDocServer>())
    .Build();


await host.RunAsync();