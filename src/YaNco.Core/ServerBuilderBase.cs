using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

/// <summary>
/// Base class for building a <see cref="IRfcServer{RT}"/>
/// </summary>
/// <typeparam name="TBuilder">The builder type for chaining</typeparam>
/// <typeparam name="RT">Runtime type</typeparam>
[PublicAPI]
public class ServerBuilderBase<TBuilder,RT> : RfcBuilderBase<TBuilder, RT>
    where RT : struct, HasSAPRfcServer<RT>,
    HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasEnvRuntimeSettings
    where TBuilder: ServerBuilderBase<TBuilder, RT>

{
    private readonly IDictionary<string, string> _serverParam;
    [CanBeNull] private IDictionary<string, string> _clientParam;
    private Action<RfcServerClientConfigurer<RT>> _configureServerClient = _ => { };
    private readonly IFunctionRegistration _functionRegistration = new ScopedFunctionRegistration();

    private Func<IDictionary<string, string>, RT, Eff<RT, IRfcServer<RT>>>
        _serverFactory = RfcServer<RT>.Create;

    private readonly string _systemId;
    private Aff<RT, IConnection>? _connectionEffect;
    private IRfcServer<RT> _buildServer;
    private ITransactionalRfcHandler<RT> _transactionalRfcHandler;

    /// <summary>
    /// Creates a new <see cref="ServerBuilder{RT}"/> with the connection parameters supplied
    /// </summary>
    /// <param name="serverParam"></param>
    /// <exception cref="ArgumentException"></exception>
    public ServerBuilderBase(IDictionary<string, string> serverParam)
    {
        serverParam = serverParam.ToDictionary(kv => kv.Key.ToUpperInvariant(), kv => kv.Value);

        if (!serverParam.ContainsKey("SYSID"))
            throw new ArgumentException("server configuration has to contain parameter SYSID", nameof(serverParam));

        _systemId = serverParam["SYSID"];
        _serverParam = serverParam;
        Self = (TBuilder) this;
    }

    /// <summary>
    /// Use a alternative factory method to create the <see cref="IRfcServer{RT}"/>. 
    /// </summary>
    /// <param name="factory">factory method</param>
    /// <returns><typeparamref name="TBuilder"/> for chaining</returns>
    /// <remarks>The default implementation call <see cref="RfcServer{RT}.Create"/>.
    /// </remarks>
    public TBuilder UseFactory(Func<IDictionary<string, string>, RT, Eff<RT, IRfcServer<RT>>> factory)
    {
        _serverFactory = factory;
        return (TBuilder) this;
    }

    /// <summary>
    /// Configures the client connection of the <see cref="IRfcServer{RT}"/>
    /// </summary>
    /// <param name="connectionParams">connection parameters</param>
    /// <param name="configure">action to configure connection</param>
    /// <returns><typeparamref name="TBuilder"/> for chaining</returns>
    public TBuilder WithClientConnection(
        IDictionary<string, string> connectionParams, Action<RfcServerClientConfigurer<RT>> configure)
    {
        _clientParam = connectionParams;
        _configureServerClient = configure;
        return (TBuilder) this;
    }

    /// <summary>
    /// Adds a existing client connection factory as client connection of <see cref="IRfcServer{RT}"/>
    /// </summary>
    /// <param name="connectionEffect">connection IO effect to use</param>
    /// <returns>current instance for chaining.</returns>
    /// <remarks>This method signature should only be used carefully.
    /// The created function may have a different runtime instance or other settings that
    /// are not configured automatically on <see cref="IRfcServer{RT}"/>.
    /// Consider using the configured client connection with
    /// <see cref="WithClientConnection(IDictionary{string,string},Action{Dbosoft.YaNco.RfcServerClientConfigurer{RT}})"/>
    /// </remarks>
    /// <returns><typeparamref name="TBuilder"/> for chaining</returns>
    public TBuilder WithClientConnection(
        Aff<RT, IConnection> connectionEffect)
    {
        _connectionEffect = connectionEffect;
        return (TBuilder)this;
    }

    /// <summary>
    /// Adds a transaction handler instance and enables transactional RFC for the server. 
    /// </summary>
    /// <param name="transactionalRfcHandler"></param>
    /// <returns><typeparamref name="TBuilder"/> for chaining</returns>
    public TBuilder WithTransactionalRfc(ITransactionalRfcHandler<RT> transactionalRfcHandler)
    {
        _transactionalRfcHandler = transactionalRfcHandler;
        return  (TBuilder)this;
    }

    /// <summary>
    /// This methods builds the async effect to create the <see cref="IRfcServer{RT}"/>
    /// </summary>
    /// <remarks>
    /// Multiple calls of this method will return the same effect. 
    /// </remarks>
    /// <returns><see cref="Aff{RT,A}"/> with the <see cref="IRfcServer{RT}"/></returns>
    public Aff<RT, IRfcServer<RT>> Build()
    {
        if (_buildServer != null)
            return Prelude.SuccessAff(_buildServer);

        switch (_connectionEffect)
        {
            //build connection from client build if necessary
            case null when _clientParam != null:
            {
                var clientBuilder = new ConnectionBuilder<RT>(_clientParam);

                //take control of registration made by clients
                clientBuilder.WithFunctionRegistration(_functionRegistration);

                _configureServerClient(new RfcServerClientConfigurer<RT>(clientBuilder));

                WithClientConnection(clientBuilder.Build());
                break;
            }
        }

        return
            from rt in Prelude.runtime<RT>()
            from serverIO in rt.RfcServerEff
            from server in _serverFactory(_serverParam, rt)
                .Map(s =>
                {
                    if (_connectionEffect != null)
                        s.AddClientConnection(_connectionEffect.Value);
                    return s;
                })

                // add transactional handler
                .Bind(server =>
                {
                    if (_transactionalRfcHandler == null)
                        return Prelude.SuccessEff(server);

                    return serverIO.AddTransactionHandlers(_systemId,
                        (handle, tid) => RunTHandler(rt, ()=> _transactionalRfcHandler.OnCheck(handle, tid)),
                        (handle, tid) => RunTHandler(rt, () => _transactionalRfcHandler.OnCommit(handle, tid)),
                        (handle, tid) => RunTHandler(rt, () => _transactionalRfcHandler.OnRollback(handle, tid)),
                        (handle, tid) => RunTHandler(rt, () => _transactionalRfcHandler.OnConfirm(handle, tid))
                    ).Map(holder =>
                    {
                        server.AddReferences(new[] { holder });
                        return server;
                    }).ToEff(l => l);
                })

                // add function handlers
                .Bind(RegisterFunctionHandlers)
                .Map(server =>
                {
                    server.AddReferences(new[] { _functionRegistration });
                    _buildServer = server;
                    return server;
                })
        select server;
    }

    private static RfcRc RunTHandler(RT runtime, Func<Eff<RT, RfcRc>> handler) => 
        handler().Run(runtime).IfFail(RfcRc.RFC_EXTERNAL_FAILURE);

    private Eff<RT,IRfcServer<RT>> RegisterFunctionHandlers(IRfcServer<RT> server)
    {
        return FunctionHandlers.Map(reg =>
        {
            if (_functionRegistration.IsFunctionRegistered(_systemId, reg.Item1))
                return Prelude.SuccessEff(Unit.Default);


            var (functionName, configureBuilder, callBackFunction) = reg;

            var builder = new FunctionBuilder<RT>(functionName);
            configureBuilder(builder);
            return from description in builder.Build()
                from rt in Prelude.runtime<RT>()
                from functionsIO in default(RT).RfcFunctionsEff
                from uAdd in functionsIO.AddFunctionHandler(_systemId,
                        description,
                        (rfcHandle, f) => callBackFunction(
                            new CalledFunction<RT>(rfcHandle, f, () => new RfcServerContext<RT>(server))).ToEither(rt))
                    .Map(holder =>
                    {
                        _functionRegistration.Add(reg.Item1, functionName, holder);
                        return Unit.Default;
                    }).ToEff(l=>l)
                select Unit.Default;

        }).Traverse(l => l).Map(_ => server);

    }

}