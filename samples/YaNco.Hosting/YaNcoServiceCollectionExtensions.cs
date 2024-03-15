using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco;
using Dbosoft.YaNco.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace YaNco.Hosting
{
    [ExcludeFromCodeCoverage]
    public static class YaNcoServiceCollectionExtensions
    {

        public static IServiceCollection AddYaNco(this IServiceCollection services)
        {
            services.AddSingleton<ILogger, RfcLoggingAdapter>();
            services.AddSingleton<SAPConnectionFactory>();
            services.AddSingleton(sp => sp.GetRequiredService<SAPConnectionFactory>().CreateConnectionFunc());
            services.AddTransient<IRfcContext, RfcContext>();

            return services;
        }
    }
}