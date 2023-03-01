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

        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IConnection>>
            _connectionFactory = Connection.Create;


        private Func<EitherAsync<RfcErrorInfo, IConnection>> _buildFunction;

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
            Func<CalledFunction, EitherAsync<RfcErrorInfo, Unit>> calledFunc)
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
            Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IConnection>> factory)
        {
            _connectionFactory = factory;
            return this;
        }

        /// <summary>
        /// This method Builds the connection function from the <see cref="ConnectionBuilder"/> settings.
        /// </summary>
        /// <returns>current instance for chaining.</returns>
        /// <remarks>
        /// The connection builder first creates RfcRuntime and calls any registered runtime configure action.
        /// The result is a function that first calls the connection factory (defaults to <seealso cref="Connection.Create"/>
        /// and afterwards registers function handlers.
        /// </remarks>
        public Func<EitherAsync<RfcErrorInfo, IConnection>> Build()
        {
            if(_buildFunction != null)
                return _buildFunction;
            
            _buildFunction = () => _connectionFactory(_connectionParam, CreateRfcRuntime())
                .Bind(RegisterFunctionHandlers);

            return _buildFunction;

        }

        private EitherAsync<RfcErrorInfo, IConnection> RegisterFunctionHandlers(IConnection connection)
        {
            return connection.GetAttributes().Bind(attributes =>
            {
                return FunctionHandlers.Map(reg =>
                {
                    var (functionName, configureBuilder, callBackFunction) = reg;

                    if (connection.RfcRuntime.IsFunctionHandlerRegistered(attributes.SystemId, functionName))
                        return Unit.Default;

                    if (configureBuilder != null)
                    {
                        var builder = new FunctionBuilder(connection.RfcRuntime, functionName);
                        configureBuilder(builder);
                        return builder.Build().ToAsync().Bind(descr =>
                        {
                            return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId,
                                functionName,
                                descr,
                                (rfcHandle, f) => callBackFunction(new CalledFunction(connection.RfcRuntime, rfcHandle, f, 
                                    () => new RfcContext(Build())))).ToAsync();
                        });

                    }

                    return connection.CreateFunction(functionName).Bind(func =>
                    {
                        return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId,
                            functionName,
                            func,
                            (rfcHandle, f) => callBackFunction(
                                new CalledFunction(connection.RfcRuntime, rfcHandle, f, () => new RfcContext(Build())))).ToAsync();
                    });
                }).Traverse(l => l).Map(eu => connection);
            });
        }
    }

    public abstract class RfcBuilderBase<TBuilder>
    {
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => { };
        protected TBuilder Self { get; set; }

        protected readonly List<(string, Action<IFunctionBuilder>,
            Func<CalledFunction, EitherAsync<RfcErrorInfo, Unit>>)> FunctionHandlers
            = new List<(string, Action<IFunctionBuilder>, Func<CalledFunction, EitherAsync<RfcErrorInfo, Unit>>)>();


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
        /// To register a known function use the signature with function name <seealso cref="WithFunctionHandler(string,System.Func{Dbosoft.YaNco.CalledFunction,LanguageExt.Either{Dbosoft.YaNco.RfcErrorInfo,LanguageExt.Unit}})"/>
        /// Function handlers are registered process wide (in the SAP NW RFC Library) and mapped to backend system id. 
        /// Multiple registrations of same function and same backend id will therefore have no effect.
        /// </remarks>
        public TBuilder WithFunctionHandler(string functionName,
            Action<IFunctionBuilder> configureBuilder,
            Func<CalledFunction, EitherAsync<RfcErrorInfo, Unit>> calledFunc)
        {
            FunctionHandlers.Add((functionName, configureBuilder, calledFunc));
            return Self;
        }


        protected IRfcRuntime CreateRfcRuntime()
        {
            var runtimeConfigurer = new RfcRuntimeConfigurer();
            _configureRuntime(runtimeConfigurer);
            return runtimeConfigurer.Create();
            
        }
    }
}
