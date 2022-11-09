using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WebApi;
using YaNco.Hosting;

namespace SAPWebAPI
{
    [ExcludeFromCodeCoverage]
    public class Program
    {
        public static void Main(string[] args)
        {
            RfcLibraryHelper.EnsurePathVariable();

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
