using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class Connection : IConnection
    {
        private readonly IAgent<AgentMessage, Either<RfcErrorInfo, object>> _stateAgent;
        private bool _disposed;

        public Connection(
            IConnectionHandle connectionHandle, 
            IRfcRuntime rfcRuntime)
        {

            _stateAgent = Agent.Start<IConnectionHandle, AgentMessage, Either<RfcErrorInfo, object>>(
                connectionHandle, (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "Connection already destroyed", "", "", "", "", "", "", "", ""));

                    try
                    {
                        switch (msg)
                        {
                            case CreateFunctionMessage createFunctionMessage:
                            {
                                var result = rfcRuntime.GetFunctionDescription(handle, createFunctionMessage.FunctionName).Use(
                                    used => used.Bind(rfcRuntime.CreateFunction)).Map(f => (object) new Function(f,rfcRuntime));

                                return (handle, result);

                            }

                            case InvokeFunctionMessage invokeFunctionMessage:
                            {
                               
                                var result = rfcRuntime.Invoke(handle, invokeFunctionMessage.Function.Handle).Map(u => (object) u);
                                return (handle, result);

                            }

                            case AllowStartOfProgramsMessage allowStartOfProgramsMessage:
                            {
                                var result = rfcRuntime.AllowStartOfPrograms(handle,
                                    allowStartOfProgramsMessage.Callback).Map(u => (object)u);
                                return (handle, result) ;

                            }

                            case DisposeMessage _:
                            {
                                handle.Dispose();
                                return (null, new Either<RfcErrorInfo, object>());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        rfcRuntime.Logger.IfSome(l => l.LogException(ex));
                    }

                    throw new InvalidOperationException();
                });
        }

        public static EitherAsync<RfcErrorInfo,IConnection> Create(IDictionary<string, string> connectionParams, IRfcRuntime runtime)
        {
            return runtime.OpenConnection(connectionParams).ToAsync().Map(handle => (IConnection) new Connection(handle, runtime));
        }

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait()
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => f.SetField("WAIT", "X").Map(_=>f).ToAsync())
                .Bind(f => f.Apply(InvokeFunction).Map(u => f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        public EitherAsync<RfcErrorInfo, Unit> Commit()
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => f.Apply(InvokeFunction).Map(u=>f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback()
        {
            return CreateFunction("BAPI_TRANSACTION_ROLLBACK")
                .Bind(InvokeFunction);

        }


        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name) =>
            _stateAgent.Tell(new CreateFunctionMessage(name)).ToAsync().Map(r => (IFunction) r);

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function) =>
            _stateAgent.Tell(new InvokeFunctionMessage(function)).ToAsync().Map(r => Unit.Default);

        public EitherAsync<RfcErrorInfo, Unit> AllowStartOfPrograms(StartProgramDelegate callback) =>
            _stateAgent.Tell(new AllowStartOfProgramsMessage(callback)).ToAsync().Map(r => Unit.Default);


        private class AgentMessage
        {

        }

        private class CreateFunctionMessage : AgentMessage
        {
            public readonly string FunctionName;
            public CreateFunctionMessage(string functionName)
            {
                FunctionName = functionName;
            }

        }

        private class InvokeFunctionMessage : AgentMessage
        {
            public readonly IFunction Function;

            public InvokeFunctionMessage(IFunction function)
            {
                Function = function;
            }
        }

        private class AllowStartOfProgramsMessage : AgentMessage
        {
            public readonly StartProgramDelegate Callback;

            public AllowStartOfProgramsMessage(StartProgramDelegate callback)
            {
                Callback = callback;
            }
        }

        private class DisposeMessage : AgentMessage
        {
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            _stateAgent.Tell(new DisposeMessage());

        }
    }
}
