using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    /// <summary>
    /// Default implementation of <see cref="IConnection"/>
    /// </summary>
    public class Connection : IConnection
    {
        private readonly IConnectionHandle _connectionHandle;
        public IRfcRuntime RfcRuntime { get; }
        private readonly IAgent<AgentMessage, Either<RfcError, object>> _stateAgent;

        public bool Disposed { get; private set; }

        public Connection(
            IConnectionHandle connectionHandle, 
            IRfcRuntime rfcRuntime)
        {
            _connectionHandle = connectionHandle;
            RfcRuntime = rfcRuntime;

            _stateAgent = Agent.Start<IConnectionHandle, AgentMessage, Either<RfcError, object>>(
                connectionHandle, (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "", "Connection already destroyed", "", "E", "", "", "", "", "").ToRfcError());

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

                            case CreateStructureMessage createStructureMessage:
                            {
                                var result = rfcRuntime.GetTypeDescription(handle, createStructureMessage.StructureName).Use(
                                    used => used.Bind(rfcRuntime.CreateStructure)).Map(h => (object)new Structure(h, rfcRuntime));

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
                        return (null, Prelude.Left(RfcError.Error(ex.Message)));
                    }

                    rfcRuntime.Logger.IfSome(l => l.LogError(
                        $"Invalid rfc connection message {msg.GetType()}"));
                    return (null, Prelude.Left(RfcError.Error($"Invalid rfc connection message {msg.GetType().Name}")));

                });
        }

        /// <summary>
        /// Creates a new connection from connection parameters
        /// </summary>
        /// <param name="connectionParams">parameters of the connection</param>
        /// <param name="runtime">Runtime used to create the connection</param>
        /// <returns></returns>
        public static EitherAsync<RfcError,IConnection> Create(IDictionary<string, string> connectionParams, IRfcRuntime runtime)
        {
            return runtime.OpenConnection(connectionParams).ToAsync().Map(handle => (IConnection) new Connection(handle, runtime));
        }

        /// <inheritdoc cref="CommitAndWait()"/>
        public EitherAsync<RfcError, Unit> CommitAndWait()
        {
            return CommitAndWait(CancellationToken.None);
        }

        /// <inheritdoc cref="CommitAndWait(CancellationToken)"/>
        public EitherAsync<RfcError, Unit> CommitAndWait(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => f.SetField("WAIT", "X").Map(_=>f).ToAsync())
                .Bind(f => InvokeFunction(f, cancellationToken).Map(u => f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        /// <inheritdoc cref="Commit()"/>
        public EitherAsync<RfcError, Unit> Commit()
        {
            return Commit(CancellationToken.None);
        }

        /// <inheritdoc cref="Commit(CancellationToken)"/>
        public EitherAsync<RfcError, Unit> Commit(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_COMMIT")
                .Bind(f => InvokeFunction(f,cancellationToken).Map(u=>f))
                .HandleReturn()
                .Map(f => Unit.Default);

        }

        /// <inheritdoc cref="Rollback()"/>
        public EitherAsync<RfcError, Unit> Rollback()
        {
            return Rollback(CancellationToken.None);
        }

        /// <inheritdoc cref="Rollback(CancellationToken)"/>
        public EitherAsync<RfcError, Unit> Rollback(CancellationToken cancellationToken)
        {
            return CreateFunction("BAPI_TRANSACTION_ROLLBACK")
                .Bind(f=> InvokeFunction(f, cancellationToken));

        }

        /// <inheritdoc cref="Cancel()"/>
        public EitherAsync<RfcError, Unit> Cancel()
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

        /// <inheritdoc cref="CreateStructure(string)"/>
        public EitherAsync<RfcError, IStructure> CreateStructure(string name) =>
            _stateAgent.Tell(new CreateStructureMessage(name)).ToAsync().Map(r => (IStructure)r);

        /// <inheritdoc cref="CreateFunction(string)"/>
        public EitherAsync<RfcError, IFunction> CreateFunction(string name) =>
            _stateAgent.Tell(new CreateFunctionMessage(name)).ToAsync().Map(r => (IFunction) r);

        /// <inheritdoc cref="InvokeFunction(IFunction)"/>
        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function)
        {
            return InvokeFunction(function,CancellationToken.None);

        }

        /// <inheritdoc cref="InvokeFunction(IFunction, CancellationToken)"/>
        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken) 
            => _stateAgent.Tell(new InvokeFunctionMessage(function, cancellationToken)).ToAsync().Map(_ => Unit.Default);

        [Obsolete("Use method WithStartProgramCallback of ConnectionBuilder instead. This method will be removed in next major release.")]
        [ExcludeFromCodeCoverage]
        public EitherAsync<RfcError, Unit> AllowStartOfPrograms(StartProgramDelegate callback)
        {
            return RfcRuntime.AllowStartOfPrograms(_connectionHandle, callback).ToAsync();
        }

        /// <inheritdoc cref="GetAttributes()"/>
        public EitherAsync<RfcError, ConnectionAttributes> GetAttributes()
        {
            return RfcRuntime.GetConnectionAttributes(_connectionHandle).ToAsync();
        }

        private class AgentMessage
        {

        }

        private class CreateStructureMessage : AgentMessage
        {
            public readonly string StructureName;
            public CreateStructureMessage(string structureName)
            {
                StructureName = structureName;
            }

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


        private class DisposeMessage : AgentMessage
        {
            public readonly RfcError ErrorInfo;

            public DisposeMessage(RfcError errorInfo)
            {
                ErrorInfo = errorInfo;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                return;
            
            Disposed = true;
            _stateAgent.Tell(new DisposeMessage(RfcError.EmptyResult));

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

}
