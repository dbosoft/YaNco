using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class Connection : IConnection
    {
        private readonly IConnectionHandle _connectionHandle;
        private readonly IRfcRuntime _rfcRuntime;
        private readonly IAgent<AgentMessage, Either<RfcErrorInfo, object>> _stateAgent;
        public bool Disposed { get; private set; }
        private bool _functionCalled;

        public Connection(
            IConnectionHandle connectionHandle, 
            IRfcRuntime rfcRuntime)
        {
            _connectionHandle = connectionHandle;
            _rfcRuntime = rfcRuntime;

            _stateAgent = Agent.Start<IConnectionHandle, AgentMessage, Either<RfcErrorInfo, object>>(
                connectionHandle, (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "", "Connection already destroyed", "", "E", "", "", "", "", ""));

                    if(!(msg is DisposeMessage))
                    {
                        rfcRuntime.IsConnectionHandleValid(handle).IfLeft(l => { msg = new DisposeMessage(l); });
                    }

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
                                _functionCalled = true;
                                StartWaitForFunctionCancellation(invokeFunctionMessage.CancellationToken);
                                try
                                {
                                    var result = rfcRuntime.Invoke(handle, invokeFunctionMessage.Function.Handle)
                                        .Map(u => (object) u);
                                    return (handle, result);
                                }
                                finally
                                {
                                    _functionCalled = false;
                                }
                            }

                            case AllowStartOfProgramsMessage allowStartOfProgramsMessage:
                            {
                                var result = rfcRuntime.AllowStartOfPrograms(handle,
                                    allowStartOfProgramsMessage.Callback).Map(u => (object)u);
                                return (handle, result) ;

                            }

                            case DisposeMessage disposeMessage:
                            {
                                handle.Dispose();
                                return (null, Prelude.Left(disposeMessage.ErrorInfo));
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
            return CommitAndWait(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => f.SetField("WAIT", "X").Map(_=>f).ToAsync())
                .Bind(f => InvokeFunction(f, cancellationToken).Map(u => f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        public EitherAsync<RfcErrorInfo, Unit> Commit()
        {
            return Commit(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => InvokeFunction(f,cancellationToken).Map(u=>f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback()
        {
            return Rollback(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_ROLLBACK")
                .Bind(f=> InvokeFunction(f, cancellationToken));

        }

        public EitherAsync<RfcErrorInfo, Unit> Cancel()
        {
            var res = _rfcRuntime.CancelConnection(_connectionHandle).ToAsync();
            Dispose();
            return res;
        }

        private async void StartWaitForFunctionCancellation(CancellationToken token)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Run(() =>
            {
                while (!token.IsCancellationRequested && _functionCalled)
                {
                    if(token.WaitHandle.WaitOne(1000))
                        break;

                }
            }).ConfigureAwait(false);

            if (token.IsCancellationRequested && _functionCalled)
                await Cancel().ToEither().ConfigureAwait(false); 
        }
    
        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name) =>
            _stateAgent.Tell(new CreateFunctionMessage(name)).ToAsync().Map(r => (IFunction) r);

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function)
        {
            return InvokeFunction(function,CancellationToken.None);

        }

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken) 
            => _stateAgent.Tell(new InvokeFunctionMessage(function, cancellationToken)).ToAsync().Map(_ => Unit.Default);

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
            public CancellationToken CancellationToken { get; }
            public readonly IFunction Function;

            public InvokeFunctionMessage(IFunction function, CancellationToken cancellationToken)
            {
                CancellationToken = cancellationToken;
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
            public readonly RfcErrorInfo ErrorInfo;

            public DisposeMessage(RfcErrorInfo errorInfo)
            {
                ErrorInfo = errorInfo;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            
            Disposed = true;
            _stateAgent.Tell(new DisposeMessage(RfcErrorInfo.EmptyResult()));

        }
    }
}
