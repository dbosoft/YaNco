using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LanguageExt;
using Microsoft.Extensions.Configuration;

namespace Dbosoft.YaNco.Hosting
{
    [ExcludeFromCodeCoverage]
    public class SAPConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public SAPConnectionFactory(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Dictionary<string, string> CreateSettings()
        {
            var config = new Dictionary<string, string>();
            _configuration.Bind("saprfc", config);

            return config;
        }

        public Func<EitherAsync<RfcError, IConnection>> CreateConnectionFunc()
        {
            var builder = new ConnectionBuilder(CreateSettings())
                .ConfigureRuntime(c =>
                    c.UseSettingsFactory( (_,m,o) => new SAPRfcRuntimeSettings(_logger, m, o) ));

            return builder.Build();

        }
    }
}