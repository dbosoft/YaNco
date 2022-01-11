using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ConnectionBuilder
    {
        private readonly IDictionary<string, string> _connectionParam;
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => {};
        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo,IConnection>> 
            _connectionFactory = Connection.Create;

        public ConnectionBuilder(IDictionary<string, string> connectionParam)
        {
            _connectionParam = connectionParam;
        }

        public ConnectionBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer> configure)
        {
            _configureRuntime = configure;
            return this;
        }

        [Obsolete("Use method AllowStartOfPrograms on RfcRuntimeConfigurer (ConfigureRuntime) instead. This method will be removed in next major release.")]
        public ConnectionBuilder WithStartProgramCallback(StartProgramDelegate startProgramDelegate)
        {
            return ConfigureRuntime(c => c.AllowStartOfPrograms(startProgramDelegate));
        }

        public ConnectionBuilder UseFactory(
            Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IConnection>> factory)
        {
            _connectionFactory = factory;
            return this;
        }

        public Func<EitherAsync<RfcErrorInfo, IConnection>> Build()
        {
            var runtimeConfigurer = new RfcRuntimeConfigurer();
            _configureRuntime(runtimeConfigurer);
            var runtime = runtimeConfigurer.Create();
            
            return () => _connectionFactory(_connectionParam, runtime);

        }
    }
}
