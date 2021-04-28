using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ConnectionBuilder
    {
        private readonly IDictionary<string, string> _connectionParam;
        private StartProgramDelegate _startProgramDelegate;
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

        public ConnectionBuilder WithStartProgramCallback(StartProgramDelegate startProgramDelegate)
        {
            _startProgramDelegate = startProgramDelegate;
            return this;
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
            
            if(_startProgramDelegate == null)
                return () => _connectionFactory(_connectionParam, runtime);


            return () => (from c in _connectionFactory(_connectionParam, runtime)
                from _ in c.AllowStartOfPrograms(_startProgramDelegate)
                select c);
        }
    }
}
