using Microsoft.Extensions.Hosting;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.Threading.Tasks;
using YaNco.Hosting;

namespace CreateSalesOrder
{
    internal class Program
    {
        // ReSharper disable once ArrangeTypeMemberModifiers
        static async Task Main(string[] args)
        {
            //This is a sample application that can be used to create a sales document in SAP with YaNco
            //Prerequisites: 
            // - a SAP System with SD (ERP or S/4)
            // - maintained connection settings either in appsettings.json file, or in user secrets
            // - maintained customizing settings in appsettings.json
            //   the default settings are preconfigured for a IDES system (BIKE shop)
            // - a valid customerNo and product id (customer T-BIKE01, product T-FC0100 could be used in IDES).

            //we use the System.CommandLine package for handling input
            var runner = new CommandLineBuilder(new CreateSimpleSalesDocument())

                .UseHost(_ => new HostBuilder(), (builder) => builder
                    .ConfigureDefaults(args)
                    .ConfigureServices((_, services) =>
                    {
                        services.AddYaNco(); // adds YaNco to service collection
                    })
                    .UseCommandHandler<CreateSimpleSalesDocument, CreateSimpleSalesDocument.Handler>())
                    .UseDefaults().Build();

            await runner.InvokeAsync(args);


        }
    }
}
