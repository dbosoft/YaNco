using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;
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

        public Func<EitherAsync<RfcErrorInfo, IConnection>> CreateConnectionFunc()
        {
            var builder = new ConnectionBuilder(CreateSettings())
                .ConfigureRuntime(c =>
                    c.UseFactory(_runtimeFactory));

            return builder.Build();

        }
    }
}