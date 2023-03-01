using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco
{
    /// <summary>
    /// This class is used to configure a SAP RFC server
    /// </summary>
    public class ServerBuilder : RfcBuilderBase<ServerBuilder>
    {
        private readonly IDictionary<string, string> _serverParam;
        [CanBeNull] private IDictionary<string, string> _clientParam;
        private Action<RfcServerClientConfigurer> _configureServerClient = (c) => { };

        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IRfcServer>>
            _serverFactory = RfcServer.Create;

        private readonly string _systemId;
        private Func<EitherAsync<RfcErrorInfo, IConnection>> _connectionFactory;
        private IRfcServer _buildServer;

        /// <summary>
        /// Creates a new <see cref="ServerBuilder"/> with the connection parameters supplied
        /// </summary>
        /// <param name="serverParam"></param>
        /// <exception cref="ArgumentException"></exception>
        public ServerBuilder(IDictionary<string, string> serverParam)
        {
            serverParam = serverParam.ToDictionary(kv => kv.Key.ToUpperInvariant(), kv => kv.Value);

            if (!serverParam.ContainsKey("SYSID"))
                throw new ArgumentException("server configuration has to contain parameter SYSID", nameof(serverParam));

            _systemId = serverParam["SYSID"];
            _serverParam = serverParam;
            Self = this;
        }

        /// <summary>
        /// Use a alternative factory method to create the <see cref="IRfcServer"/>. 
        /// </summary>
        /// <param name="factory">factory method</param>
        /// <returns>current instance for chaining.</returns
        /// <remarks>The default implementation call <see cref="RfcServer.Create"/>.
        /// </remarks>
        public ServerBuilder UseFactory(
            Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IRfcServer>> factory)
        {
            _serverFactory = factory;
            return this;
        }

        /// <summary>
        /// Configures the client connection of the <see cref="IRfcServer"/>
        /// </summary>
        /// <param name="connectionParams">connection parameters</param>
        /// <param name="configure">action to configure connection</param>
        /// <returns>current instance for chaining.</returns>
        public ServerBuilder WithClientConnection(IDictionary<string, string> connectionParams, Action<RfcServerClientConfigurer> configure)
        {
            _clientParam = connectionParams;
            _configureServerClient = configure;
            return this;
        }

        /// <summary>
        /// Adds a existing client connection factory as client connection of <see cref="IRfcServer"/>
        /// </summary>
        /// <param name="connectionFactory">function of connection factory</param>
        /// <returns>current instance for chaining.</returns>
        /// <remarks>This method signature should only be used in special cases.
        /// The created function may have a different <see cref="IRfcRuntime"/> or other settings that
        /// are not configured automatically on <see cref="IRfcServer"/>.
        /// Consider using the configured client connection with <see cref="WithClientConnection(IDictionary{string,string},Action{Dbosoft.YaNco.RfcServerClientConfigurer})"/>
        /// </remarks>
        public ServerBuilder WithClientConnection(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionFactory)
        {
            _connectionFactory = connectionFactory;
            return this;
        }
        
        public EitherAsync<RfcErrorInfo, IRfcServer> Build()
        {
            if (_buildServer != null)
                return Prelude.RightAsync<RfcErrorInfo, IRfcServer>(_buildServer);


            var runtime = CreateRfcRuntime();

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
                .Bind(RegisterFunctionHandlers)
                .Map(server =>
                {
                    _buildServer = server;
                    return server;
                });

        }


        private EitherAsync<RfcErrorInfo, IRfcServer> RegisterFunctionHandlers(IRfcServer server)
        {
            return FunctionHandlers.Map(reg =>
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
                        (rfcHandle, f) => callBackFunction(
                            new CalledFunction(server.RfcRuntime, rfcHandle, f, () => new RfcServerContext(server)))).ToAsync();
                });


            }).Traverse(l => l).Map(eu => server);

        }

    }
}