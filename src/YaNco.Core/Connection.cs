using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class Connection : IConnection
    {
        private readonly IConnectionHandle _connectionHandle;
        public IRfcRuntime RfcRuntime { get; }
        private readonly IAgent<AgentMessage, Either<RfcErrorInfo, object>> _stateAgent;
        public bool Disposed { get; private set; }

        public Connection(
            IConnectionHandle connectionHandle, 
            IRfcRuntime rfcRuntime)
        {
            _connectionHandle = connectionHandle;
            RfcRuntime = rfcRuntime;

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
                                using (var callContext = new FunctionCallContext())
                                {
                                    StartWaitForFunctionCancellation(callContext, invokeFunctionMessage.CancellationToken);
                                    var result = rfcRuntime.Invoke(handle, invokeFunctionMessage.Function.Handle)
                                        .Map(u => (object) u);
                                    return (handle, result);

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
            var res = RfcRuntime.CancelConnection(_connectionHandle).ToAsync();
            Dispose();
            return res;
        }

        private async void StartWaitForFunctionCancellation(FunctionCallContext context, CancellationToken token)
        {
            // ReSharper disable once MethodSupportsCancellation
            await Task.Factory.StartNew(() =>
            {
                while (!token.IsCancellationRequested && !context.Exited)
                {
                    if(token.WaitHandle.WaitOne(500,true))
                        break;

                }
            }, TaskCreationOptions.LongRunning).ConfigureAwait(false);

            if (token.IsCancellationRequested && !context.Exited)
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

        private class FunctionCallContext : IDisposable
        {
            public bool Exited { get; private set;  }
            public void Dispose()
            {
                Exited = true;
            }
        }
    }


    public static class Agent
    {
        public static IAgent<TMsg> Start<TMsg>(Action<TMsg> action, CancellationToken cancellationToken = default)
           => new StatelessAgent<TMsg>(action, cancellationToken);

        public static IAgent<TMsg> Start<TState, TMsg>
           (Func<TState> initState
           , Func<TState, TMsg, TState> process, CancellationToken cancellationToken = default)
           => new StatefulAgent<TState, TMsg>(initState(), process, cancellationToken);

        public static IAgent<TMsg> Start<TState, TMsg>
           (TState initialState
           , Func<TState, TMsg, TState> process, CancellationToken cancellationToken = default)
           => new StatefulAgent<TState, TMsg>(initialState, process, cancellationToken);

        public static IAgent<TMsg> Start<TState, TMsg>
           (TState initialState
           , Func<TState, TMsg, Task<TState>> process, CancellationToken cancellationToken = default)
           => new StatefulAgent<TState, TMsg>(initialState, process, cancellationToken);

        public static IAgent<TMsg, TReply> Start<TState, TMsg, TReply>
           (TState initialState
           , Func<TState, TMsg, (TState, TReply)> process, CancellationToken cancellationToken = default)
           => new TwoWayAgent<TState, TMsg, TReply>(initialState, process, cancellationToken);

        public static IAgent<TMsg, TReply> Start<TState, TMsg, TReply>
           (TState initialState
           , Func<TState, TMsg, Task<(TState, TReply)>> process, CancellationToken cancellationToken = default)
           => new TwoWayAgent<TState, TMsg, TReply>(initialState, process, cancellationToken);
    }

    public interface IAgent<in TMsg>
    {
        void Tell(TMsg message);
    }

    public interface IAgent<in TMsg, TReply>
    {
        Task<TReply> Tell(TMsg message);
    }

    class StatelessAgent<TMsg> : IAgent<TMsg>
    {
        private readonly ActionBlock<TMsg> _actionBlock;

        public StatelessAgent(Action<TMsg> process, CancellationToken cancellationToken)
        {
            _actionBlock = new ActionBlock<TMsg>(process, new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });
        }

        public StatelessAgent(Func<TMsg, Task> process, CancellationToken cancellationToken)
        {
            _actionBlock = new ActionBlock<TMsg>(process, new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });

        }

        public void Tell(TMsg message) => _actionBlock.Post(message);
    }

    class StatefulAgent<TState, TMsg> : IAgent<TMsg>
    {
        private TState _state;
        private readonly ActionBlock<TMsg> _actionBlock;

        public StatefulAgent(TState initialState
           , Func<TState, TMsg, TState> process, CancellationToken cancellationToken)
        {
            _state = initialState;

            _actionBlock = new ActionBlock<TMsg>(
               msg => _state = process(_state, msg),// process the message with the current state, and store the resulting new state as the current state of the agent
               new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });
        }

        public StatefulAgent(TState initialState
           , Func<TState, TMsg, Task<TState>> process, CancellationToken cancellationToken)
        {
            _state = initialState;

            _actionBlock = new ActionBlock<TMsg>(
               async msg => _state = await process(_state, msg),
               new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });
        }

        public void Tell(TMsg message) => _actionBlock.Post(message);
    }

    class TwoWayAgent<TState, TMsg, TReply> : IAgent<TMsg, TReply>
    {
        private readonly CancellationToken _cancellationToken;
        private readonly ActionBlock<(TMsg, TaskCompletionSource<TReply>)> _actionBlock;

        public TwoWayAgent(TState initialState, Func<TState, TMsg, (TState, TReply)> process, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            var state = initialState;

            _actionBlock = new ActionBlock<(TMsg, TaskCompletionSource<TReply>)>(
               t =>
               {
                   var result = process(state, t.Item1);
                   state = result.Item1;
                   t.Item2.SetResult(result.Item2);
               }, new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });
        }

        // creates a 2-way agent with an async processing func
        public TwoWayAgent(TState initialState, Func<TState, TMsg, Task<(TState, TReply)>> process, CancellationToken cancellationToken)
        {
            var state = initialState;
            _cancellationToken = cancellationToken;

            _actionBlock = new ActionBlock<(TMsg, TaskCompletionSource<TReply>)>(
               async t =>
               {

                   await process(state, t.Item1)
                        .ContinueWith(task =>
                        {
                            if (task.Status == TaskStatus.Faulted)
                                t.Item2.SetException(task.Exception);
                            else
                            {
                                state = task.Result.Item1;
                                t.Item2.SetResult(task.Result.Item2);
                            }
                        })
                       .ConfigureAwait(false);
               }, new ExecutionDataflowBlockOptions { CancellationToken = cancellationToken });
        }

        public Task<TReply> Tell(TMsg message)
        {
            var tcs = new TaskCompletionSource<TReply>();
            _actionBlock.Post((message, tcs));

            // this will help to relax the task scheduler, for some reason the task may block if directly returned
            //tcs.Task.Wait(_cancellationToken);
            return tcs.Task;
        }
    }

}
