using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ServerBuilder
    {
        private readonly IDictionary<string, string> _serverParam;
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => { };
        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IRfcServer>>
            _serverFactory = RfcServer.Create;

        readonly List<(string, Action<IFunctionBuilder>, Func<CalledFunction, Either<RfcErrorInfo, Unit>>)> _functionHandlers = new List<(string, Action<IFunctionBuilder>, Func<CalledFunction, Either<RfcErrorInfo, Unit>>)>();

        public ServerBuilder(IDictionary<string, string> serverParam)
        {
            _serverParam = serverParam;
        }

        public ServerBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer> configure)
        {
            _configureRuntime = configure;
            return this;
        }

        public ServerBuilder UseFactory(
            Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IRfcServer>> factory)
        {
            _serverFactory = factory;
            return this;
        }

        public ServerBuilder WithFunctionHandler(string functionName,
            Action<IFunctionBuilder> configureBuilder,
            Func<CalledFunction, Either<RfcErrorInfo, Unit>> calledFunc)
        {
            _functionHandlers.Add((functionName, configureBuilder, calledFunc));
            return this;
        }


        public EitherAsync<RfcErrorInfo, IRfcServer> Build()
        {
            var runtimeConfigurer = new RfcRuntimeConfigurer();
            _configureRuntime(runtimeConfigurer);
            var runtime = runtimeConfigurer.Create();

            return _serverFactory(_serverParam, runtime);

        }

    }
}