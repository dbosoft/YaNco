using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class ConnectionBuilder
    {
        private readonly IDictionary<string, string> _connectionParam;
        private Action<RfcRuntimeConfigurer> _configureRuntime = (c) => { };

        private Func<IDictionary<string, string>, IRfcRuntime, EitherAsync<RfcErrorInfo, IConnection>>
            _connectionFactory = Connection.Create;

        readonly List<(string, Action<IFunctionBuilder>,
            Func<CalledFunction, Either<RfcErrorInfo, Unit>>)> _functionHandlers
            = new List<(string, Action<IFunctionBuilder>, Func<CalledFunction, Either<RfcErrorInfo, Unit>>)>();

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
            return WithFunctionHandler("RFC_START_PROGRAM", builder => builder
                    .AddChar("COMMAND", RfcDirection.Import, 512),
                cf => cf
                    .Input(f => f.GetField<string>("COMMAND"))
                    .Process(cmd => startProgramDelegate(cmd))
                    .NoReply()
            );
        }

        public ConnectionBuilder WithFunctionHandler(string functionName,
            Func<CalledFunction, Either<RfcErrorInfo, Unit>> calledFunc)
        {
            _functionHandlers.Add((functionName, null, calledFunc));
            return this;
        }

        public ConnectionBuilder WithFunctionHandler(string functionName,
            Action<IFunctionBuilder> configureBuilder,
            Func<CalledFunction, Either<RfcErrorInfo, Unit>> calledFunc)
        {
            _functionHandlers.Add((functionName, configureBuilder, calledFunc));
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

            return () => _connectionFactory(_connectionParam, runtime)
                .Bind(RegisterFunctionHandlers);

        }

        private EitherAsync<RfcErrorInfo, IConnection> RegisterFunctionHandlers(IConnection connection)
        {
            return connection.GetAttributes().Bind(attributes =>
            {
                return _functionHandlers.Map(reg =>
                {
                    var (functionName, configureBuilder, callBackFunction) = reg;

                    if (configureBuilder != null)
                    {
                        var builder = new FunctionBuilder(connection.RfcRuntime, functionName);
                        configureBuilder(builder);
                        return builder.Build().ToAsync().Bind(descr =>
                        {
                            return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId, descr,
                                f => callBackFunction(new CalledFunction(f))).ToAsync();
                        });

                    }

                    return connection.CreateFunction(functionName).Bind(func =>
                    {
                        return connection.RfcRuntime.AddFunctionHandler(attributes.SystemId, func,
                            f => callBackFunction(new CalledFunction(f))).ToAsync();
                    });
                }).Traverse(l => l).Map(eu => connection);
            });
        }
    }
}
