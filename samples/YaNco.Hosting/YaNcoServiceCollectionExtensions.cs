using System;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco;
using Dbosoft.YaNco.Hosting;
using Dbosoft.YaNco.TypeMapping;
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
            services.AddSingleton<Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime>>(
                sp => (l, m, o) => new RfcRuntime(sp.GetService<ILogger>(), m, o));
            services.AddSingleton<SAPConnectionFactory>();
            services.AddScoped(sp => sp.GetRequiredService<SAPConnectionFactory>().GetConnectionProvider());
            services.AddTransient<IRfcContext, RfcContext>();

            return services;
        }
    }
}