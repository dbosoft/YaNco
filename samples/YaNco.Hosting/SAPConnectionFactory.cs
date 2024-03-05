using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.TypeMapping;
using Microsoft.Extensions.Configuration;

namespace Dbosoft.YaNco.Hosting
{
    [ExcludeFromCodeCoverage]
    public class SAPConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime> _runtimeFactory;
        public SAPConnectionFactory(IConfiguration configuration, Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime> runtimeFactory)
        {
            _configuration = configuration;
            _runtimeFactory = runtimeFactory;
        }

        public Dictionary<string, string> CreateSettings()
        {
            var config = new Dictionary<string, string>();
            _configuration.Bind("saprfc", config);

            return config;
        }

        public IRfcClientConnectionProvider GetConnectionProvider()
        {
            return new SAPConnection(CreateSettings()).AsRfcClient(b=>b
                .ConfigureRuntime(c =>
                    c.UseFactory(_runtimeFactory)));

        }
    }
}