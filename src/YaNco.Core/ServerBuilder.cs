using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ServerBuilder
    {
        private readonly IDictionary<string, string> _serverParam;
        [CanBeNull] private IDictionary<string, string> _clientParam;
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => { };
        private Action<RfcServerClientConfigurer> _configureServerClient = (c) => { };

        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IRfcServer>>
            _serverFactory = RfcServer.Create;

        private readonly string _systemId;
        private Func<EitherAsync<RfcErrorInfo, IConnection>> _connectionFactory;


        readonly List<(string, Action<IFunctionBuilder>, Func<CalledFunction, Either<RfcErrorInfo, Unit>>)> _functionHandlers = new List<(string, Action<IFunctionBuilder>, Func<CalledFunction, Either<RfcErrorInfo, Unit>>)>();

        public ServerBuilder(IDictionary<string, string> serverParam)
        {
            if (!serverParam.ContainsKey("SYSID"))
                throw new ArgumentException("server configuration has to contain parameter SYSID", nameof(serverParam));

            _systemId = serverParam["SYSID"];
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

        public ServerBuilder WithClientConnection(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            return this;
        }

        public ServerBuilder WithClientConnection(IDictionary<string, string> connectionParams, Action<RfcServerClientConfigurer> configure)
        {
            _clientParam = connectionParams;
            _configureServerClient = configure;
            return this;
        }


        public EitherAsync<RfcErrorInfo, IRfcServer> Build()
        {
            var runtimeConfigurer = new RfcRuntimeConfigurer();
            _configureRuntime(runtimeConfigurer);
            var runtime = runtimeConfigurer.Create();

            //build connection from client build if necessary
            if (_connectionFactory == null && _clientParam!= null)
            {
                var clientBuilder = new ConnectionBuilder(_clientParam);
                //take runtime of client
                clientBuilder.ConfigureRuntime(cfg => cfg.UseFactory((l, m, o) => runtime));

                _configureServerClient(new RfcServerClientConfigurer(clientBuilder));

                WithClientConnection(clientBuilder.Build());
            }

            return _serverFactory(_serverParam, runtime)
                .Map(s =>
                {
                    if (_connectionFactory != null)
                        s.AddConnectionFactory(_connectionFactory);
                    return s;
                })
                .Bind(RegisterFunctionHandlers);

        }


        private EitherAsync<RfcErrorInfo, IRfcServer> RegisterFunctionHandlers(IRfcServer server)
        {
            return _functionHandlers.Map(reg =>
            {
                var (functionName, configureBuilder, callBackFunction) = reg;

                if (server.RfcRuntime.IsFunctionHandlerRegistered(_systemId, functionName))
                    return Unit.Default;

                var builder = new FunctionBuilder(server.RfcRuntime, functionName);
                configureBuilder(builder);
                return builder.Build().ToAsync().Bind(descr =>
                {
                    return server.RfcRuntime.AddFunctionHandler(_systemId,
                        functionName,
                        descr,
                        f => callBackFunction(new CalledFunction(f, () => new RfcServerContext(server)))).ToAsync();
                });


            }).Traverse(l => l).Map(eu => server);

        }

    }

    public class RfcServerClientConfigurer
    {
        private readonly ConnectionBuilder _builder;

        public RfcServerClientConfigurer(ConnectionBuilder builder)
        {
            _builder = builder;
            
        }

        /// <summary>
        /// This method registers a function handler from a SAP function name. 
        /// </summary>
        /// <param name="functionName">Name of function</param>
        /// <param name="calledFunc">function handler</param>
        /// <returns>current instance for chaining</returns>
        /// <remarks>
        /// The metadata of the function is retrieved from the backend. Therefore the function
        /// must exists on the SAP backend system.
        /// Function handlers are registered process wide (in the SAP NW RFC Library).and mapped to backend system id. 
        /// Multiple registrations of same function and same backend id will therefore have no effect.
        /// </remarks>
        public RfcServerClientConfigurer WithFunctionHandler(string functionName,
            Func<CalledFunction, Either<RfcErrorInfo, Unit>> calledFunc)
        {
            _builder.WithFunctionHandler(functionName, calledFunc);
            return this;
        }

    }
}