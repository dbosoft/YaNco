using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;

namespace Dbosoft.YaNco
{
    /// <summary>
    /// This class is used to build client connections to a SAP ABAP backend.  
    /// </summary>
    public class ConnectionBuilder : RfcBuilderBase<ConnectionBuilder>
    {
        private readonly IDictionary<string, string> _connectionParam;
        private IFunctionRegistration _functionRegistration = FunctionRegistration.Instance;

        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcError, IConnection>>
            _connectionFactory = Connection.Create;


        private Func<EitherAsync<RfcError, IConnection>> _buildFunction;

        /// <summary>
        /// Creates a new connection builder. 
        /// </summary>
        /// <param name="connectionParam">Dictionary of connection parameters</param>
        public ConnectionBuilder(IDictionary<string, string> connectionParam)
        {
            _connectionParam = connectionParam.ToDictionary(kv => kv.Key.ToUpperInvariant(), kv => kv.Value);
            Self = this;
        }

        /// <summary>
        /// This method sets the function registration where functions created by the
        /// connection are registered. The default function registration is a global static reference
        /// holding function registration for entire process.
        /// Use this method to register a own instance of function registration that could be disposed on demand.
        /// </summary>
        /// <param name="functionRegistration"></param>
        /// <returns></returns>
        public ConnectionBuilder WithFunctionRegistration(IFunctionRegistration functionRegistration)
        {
            _functionRegistration = functionRegistration;
            return this;
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
        /// To register a generic function use the signature that builds from a <see cref="IFunctionBuilder"/>.
        /// Function handlers are registered process wide (in the SAP NW RFC Library) and mapped to backend system id. 
        /// Multiple registrations of same function and same backend id will therefore have no effect.
        /// </remarks>
        public ConnectionBuilder WithFunctionHandler(string functionName,
            Func<CalledFunction, EitherAsync<RfcError, Unit>> calledFunc)
        {
            FunctionHandlers.Add((functionName, null, calledFunc));
            return this;
        }


        /// <summary>
        /// Use a alternative factory method to create connection. 
        /// </summary>
        /// <param name="factory">factory method</param>
        /// <returns>current instance for chaining.</returns
        /// <remarks>The default implementation call <see cref="Connection.Create"/>.
        /// </remarks>
        public ConnectionBuilder UseFactory(
            Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcError, IConnection>> factory)
        {
            _connectionFactory = factory;
            return this;
        }

        /// <summary>
        /// This method Builds the connection function from the <see cref="ConnectionBuilder"/> settings.
        /// </summary>
        /// <returns><see cref="Func{EitherAsync{RfcError,IConnection}}"/>.</returns>
        /// <remarks>
        /// The connection builder first creates RfcRuntime and calls any registered runtime configure action.
        /// The result is a function that first calls the connection factory (defaults to <seealso cref="Connection.Create"/>
        /// and afterwards registers function handlers.
        /// </remarks>
        [Obsolete("Use method GetProvider() instead")]
        public Func<EitherAsync<RfcError, IConnection>> Build()
        {
            if(_buildFunction != null)
                return _buildFunction;
            
            _buildFunction = () => _connectionFactory(_connectionParam, CreateRfcRuntime())
                .Bind(RegisterFunctionHandlers);

            return _buildFunction;

        }

        /// <summary>
        /// This method Builds the connection function from the <see cref="ConnectionBuilder"/> settings.
        /// </summary>
        /// <returns><see cref="IRfcClientConnectionProvider"/> that holds or opens a connection.</returns>
        /// <remarks>
        /// The connection builder first creates RfcRuntime and calls any registered runtime configure action.
        /// The created connection provider holds the connection when used the first time.
        /// </remarks>
        public IRfcClientConnectionProvider GetProvider()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new RfcClientConnectionProvider(Build());
#pragma warning restore CS0618 // Type or member is obsolete

        }

        private EitherAsync<RfcError, IConnection> RegisterFunctionHandlers(IConnection connection)
        {
            return connection.GetAttributes().Bind(attributes =>
            {
                return FunctionHandlers.Map(reg =>
                {
                    var (functionName, configureBuilder, callBackFunction) = reg;

                    if(_functionRegistration.IsFunctionRegistered(attributes.SystemId, functionName))
                        return Unit.Default;

                    if (configureBuilder != null)
                    {
                        var builder = new FunctionBuilder(connection.RfcRuntime, functionName);
                        configureBuilder(builder);
                        return builder.Build().ToAsync().Bind(descr =>
                        {
                            return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId,
                                descr,
                                (rfcHandle, f) => callBackFunction(new CalledFunction(connection.RfcRuntime, rfcHandle, f, 
                                    () => new RfcContext(Build())))).ToAsync()
                                .Map(holder =>
                                {
                                    _functionRegistration.Add(attributes.SystemId, functionName, holder);
                                    return Unit.Default;
                                });
                        });

                    }

                    return connection.CreateFunction(functionName).Bind(func =>
                    {
                        return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId,
                            func,
                            (rfcHandle, f) => callBackFunction(
                                new CalledFunction(connection.RfcRuntime, rfcHandle, f, () => new RfcContext(Build())))).ToAsync()
                            .Map(holder =>
                            {
                                _functionRegistration.Add(attributes.SystemId, functionName, holder);
                                return Unit.Default;
                            });
                    });
                }).TraverseSerial(l => l).Map(eu => connection );
            });
        }
    }

    internal class RfcBuilderSync
    {
        public static object SyncObject = new();

    }

    public abstract class RfcBuilderBase<TBuilder>
    {
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => { };
        protected TBuilder Self { get; set; }
        private IRfcRuntime _rfcRuntime;

        protected readonly List<(string, Action<IFunctionBuilder>,
            Func<CalledFunction, EitherAsync<RfcError, Unit>>)> FunctionHandlers = new();


        /// <summary>
        /// Registers a action to configure the <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure">action with <see cref="RfcRuntimeConfigurer"/></param>
        /// <returns>current instance for chaining.</returns>
        /// <remarks>
        /// Multiple calls of this method will override the previous configuration action. 
        /// </remarks>
        public TBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer> configure)
        {
            _configureRuntime = configure;
            return Self;
        }

        /// <summary>
        /// This method registers a callback of type <see cref="StartProgramDelegate"/> 
        /// to handle backend requests to start local programs.
        /// </summary>
        /// <remarks>
        /// The SAP backend can call function RFC_START_PROGRAM on back destination to request
        /// clients to start local programs. This is used a lot in KPRO applications to start saphttp and sapftp.
        /// </remarks>
        /// <param name="startProgramDelegate">Delegate to callback function implementation.</param>
        /// <returns>current instance for chaining</returns>
        public TBuilder WithStartProgramCallback(StartProgramDelegate startProgramDelegate)
        {
            return WithFunctionHandler("RFC_START_PROGRAM", builder => builder
                    .AddChar("COMMAND", RfcDirection.Import, 512),
                cf => cf
                    .Input(f => f.GetField<string>("COMMAND"))
                    .Process(cmd => startProgramDelegate(cmd))
                    .NoReply()
            );
        }

        /// <summary>
        /// This method registers a function handler from a <see cref="IFunctionBuilder"/>
        /// </summary>
        /// <param name="functionName">Name of function</param>
        /// <param name="configureBuilder">action to configure function builder</param>
        /// <param name="calledFunc">function handler</param>
        /// <returns>current instance for chaining</returns>
        /// <remarks>
        /// The metadata of the function is build in the <see cref="IFunctionBuilder"/>. This allows to register
        /// any kind of function. 
        /// To register a known function use the signature with function name <seealso cref="WithFunctionHandler(string,System.Func{Dbosoft.YaNco.CalledFunction,LanguageExt.Either{Dbosoft.YaNco.RfcError,LanguageExt.Unit}})"/>
        /// Function handlers are registered process wide (in the SAP NW RFC Library) and mapped to backend system id. 
        /// Multiple registrations of same function and same backend id will therefore have no effect.
        /// </remarks>
        public TBuilder WithFunctionHandler(string functionName,
            Action<IFunctionBuilder> configureBuilder,
            Func<CalledFunction, EitherAsync<RfcError, Unit>> calledFunc)
        {
            FunctionHandlers.Add((functionName, configureBuilder, calledFunc));
            return Self;
        }


        protected IRfcRuntime CreateRfcRuntime()
        {
            if(_rfcRuntime!= null) return _rfcRuntime;

            lock (RfcBuilderSync.SyncObject)
            {
                var runtimeConfigurer = new RfcRuntimeConfigurer();
                _configureRuntime(runtimeConfigurer);
                _rfcRuntime = runtimeConfigurer.Create();
                return _rfcRuntime;
            }
        }
    }
}
