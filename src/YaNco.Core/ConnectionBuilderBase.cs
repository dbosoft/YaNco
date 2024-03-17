using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;

namespace Dbosoft.YaNco;

/// <summary>
/// This class is used to build client connections to a SAP ABAP backend. 
/// </summary>
/// <typeparam name="TBuilder">The builder type for chaining</typeparam>
/// <typeparam name="RT">Runtime type</typeparam>
public class ConnectionBuilderBase<TBuilder, RT> : RfcBuilderBase<TBuilder, RT>
    where TBuilder: ConnectionBuilderBase<TBuilder, RT>
    where RT : struct,
    HasSAPRfcFunctions<RT>,
    HasSAPRfcServer<RT>,
    HasSAPRfcConnection<RT>,
    HasSAPRfcLogger<RT>,
    HasSAPRfcData<RT>,
    IHasEnvRuntimeSettings
{
    private readonly IDictionary<string, string> _connectionParam;
    private IFunctionRegistration _functionRegistration = FunctionRegistration.Instance;

    private Aff<RT, IConnection>? _buildFunction;
    private Func<IDictionary<string, string>, RT, Eff<RT, IConnection>>
        _connectionFactory = Connection<RT>.Create;

    /// <summary>
    /// Creates a new connection builder. 
    /// </summary>
    /// <param name="connectionParam">Dictionary of connection parameters</param>
    public ConnectionBuilderBase(IDictionary<string, string> connectionParam)
    {
        _connectionParam = connectionParam.ToDictionary(kv => kv.Key.ToUpperInvariant(), kv => kv.Value);
        Self = (TBuilder)this;
    }

    /// <summary>
    /// Use a alternative factory method to create connection. 
    /// </summary>
    /// <param name="factory">factory method</param>
    /// <returns>current instance of <typeparamref name="TBuilder"/> for chaining.</returns>
    /// <remarks>The default implementation call <see cref="Connection{RT}.Create"/>.
    /// </remarks>
    public TBuilder UseFactory(
        Func<IDictionary<string, string>, RT, Eff<RT, IConnection>> factory)
    {
        _connectionFactory = factory;
        return (TBuilder) this;
    }
    /// <summary>
    /// This method sets the function registration where functions created by the
    /// connection are registered. The default function registration is a global static reference
    /// holding function registration for entire process.
    /// Use this method to register a own instance of function registration that could be disposed on demand.
    /// </summary>
    /// <param name="functionRegistration"></param>
    /// <returns></returns>
    public TBuilder WithFunctionRegistration(IFunctionRegistration functionRegistration)
    {
        _functionRegistration = functionRegistration;
        return (TBuilder)this;
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
    /// To register a generic function use the signature that builds from a <see cref="IFunctionBuilder{RT}"/>.
    /// Function handlers are registered process wide (in the SAP NW RFC Library) and mapped to backend system id. 
    /// Multiple registrations of same function and same backend id will therefore have no effect.
    /// </remarks>
    public TBuilder WithFunctionHandler(string functionName,
        Func<CalledFunction<RT>, Aff<RT, Unit>> calledFunc)
    {
        FunctionHandlers.Add((functionName, null, calledFunc));
        return (TBuilder)this;
    }


    /// <summary>
    /// This method Builds the connection IO effect from the <see cref="ConnectionBuilder"/> settings.
    /// </summary>
    /// <returns><see cref="Aff{RT,A}"/> with <see cref="IConnection"/></returns>
    /// <remarks>
    /// The result is a effect that first opens a connection and afterwards registers function handlers.
    /// </remarks>
    public Aff<RT, IConnection> Build()
    {
        if (_buildFunction != null)
            return _buildFunction.Value;

        _buildFunction =
            from rt in Prelude.runtime<RT>()
            from connection in _connectionFactory(_connectionParam, rt)
            from withHandlers in RegisterFunctionHandlers(connection)
            select withHandlers;

        return _buildFunction.Value;

    }


    private Aff<RT, IConnection> RegisterFunctionHandlers(IConnection connection)
    {
        return
            from rt in Prelude.runtime<RT>()
            from functionsIO in rt.RfcFunctionsEff
            from attributes in connection.GetAttributes().ToAff(l => l)
            from attach in FunctionHandlers.Map(reg =>
            {
                var (functionName, configureBuilder, callBackFunction) = reg;

                if (_functionRegistration.IsFunctionRegistered(attributes.SystemId, functionName))
                    return Prelude.unitAff;

                if (configureBuilder == null)
                    return from functionDescription in connection.CreateFunction(functionName).ToAff(l => l)
                        from uAdd in functionsIO.AddFunctionHandler(attributes.SystemId,
                                functionDescription,
                                (rfcHandle, f) => callBackFunction(
                                        new CalledFunction<RT>(rfcHandle, f, () => new RfcContext<RT>(Build())))
                                    .ToEither(rt)).ToAff(l => l)
                            .Map(holder =>
                            {
                                _functionRegistration.Add(attributes.SystemId, functionName, holder);
                                return Unit.Default;
                            })
                        select uAdd;

                var builder = new FunctionBuilder<RT>(functionName);
                configureBuilder(builder);
                return
                    from functionDescription in builder.Build()
                    from addHandler in functionsIO.AddFunctionHandler(attributes.SystemId,
                            functionDescription,
                            (rfcHandle, f) => callBackFunction(new CalledFunction<RT>(rfcHandle, f,
                                () => new RfcContext<RT>(Build()))).ToEither(rt))
                        .Map(holder =>
                        {
                            _functionRegistration.Add(attributes.SystemId, functionName, holder);
                            return Unit.Default;
                        }).ToAff(l => l)
                    select addHandler;

            }).TraverseSerial(l => l).Map(_ => connection)
            select attach;
    }

}