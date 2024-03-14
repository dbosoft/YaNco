using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.Functional;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco
{

    /// <summary>
    /// Default implementation of <see cref="IConnection"/>
    /// </summary>
    public class Connection<RT, TSettings> : IConnection
        where RT : struct,HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasEnvSettings<TSettings> 
        where TSettings : SAPRfcRuntimeSettings
    {
        private readonly RT _runtime;
        private readonly IConnectionHandle _connectionHandle;
        private readonly IAgent<AgentMessage, Either<RfcError, object>> _stateAgent;

        public bool Disposed { get; private set; }


        [Obsolete("RfcRuntime is obsolete")]
        public IRfcRuntime RfcRuntime => new RfcRuntime(SAPRfcRuntime.New(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(
            _runtime.Env.Source, _runtime.Env.Settings)));

        public T GetRuntimeSettings<T>() where T : SAPRfcRuntimeSettings
        {
            return _runtime.Env.Settings as T;
        }

        public Connection(
            RT runtime,
            IConnectionHandle connectionHandle)
        {
            _runtime = runtime;
            _connectionHandle = connectionHandle;
            _stateAgent = Agent.Start<IConnectionHandle, AgentMessage, Either<RfcError, object>>(
                connectionHandle, (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "", "Connection already destroyed", "", "E", "", "", "", "", "").ToRfcError());

                    var effect = from io in default(RT).RfcConnectionEff
                        from functionsIO in default(RT).RfcFunctionsEff
                        from dataIO in default(RT).RfcDataEff
                        from logger in default(RT).RfcLoggerEff
                        from proccessed in Prelude.Eff( () =>
                        {
                            if (!(msg is DisposeMessage))
                            {
                                io.IsConnectionHandleValid(handle).IfLeft(l => { msg = new DisposeMessage(l); });
                            }

                            try
                            {
                                switch (msg)
                                {
                                    case CreateFunctionMessage createFunctionMessage:
                                    {
                                        var result = functionsIO
                                            .GetFunctionDescription(handle, createFunctionMessage.FunctionName).Use(
                                                used => used.Bind(functionsIO.CreateFunction)).Map(f =>
                                                (object)new Function(f, dataIO, functionsIO));

                                        return (handle, result);

                                    }

                                    case CreateStructureMessage createStructureMessage:
                                    {
                                        var result = dataIO
                                            .GetTypeDescription(handle, createStructureMessage.StructureName).Use(
                                                used =>
                                                    used.Bind(dataIO.CreateStructure))
                                            .Map(h => (object)new Structure(h, dataIO));

                                        return (handle, result);

                                    }

                                    case InvokeFunctionMessage invokeFunctionMessage:
                                    {
                                        using var callContext = new FunctionCallContext();
                                        StartWaitForFunctionCancellation(callContext,
                                            invokeFunctionMessage.CancellationToken);
                                        var result = functionsIO.Invoke(handle, invokeFunctionMessage.Function.Handle)
                                            .Map(u => (object)u);
                                        return (handle, result);
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
                                logger.IfSome(l => l.LogException(ex));
                                return (null, Prelude.Left(RfcError.Error(ex.Message)));
                            }

                            logger.IfSome(l => l.LogError(
                                $"Invalid rfc connection message {msg.GetType()}"));
                            return (null,
                                Prelude.Left(RfcError.Error($"Invalid rfc connection message {msg.GetType().Name}")));


                        })
                        select proccessed;

                    var res = effect.Run(runtime).Match(
                        Fail: e => (handle, Prelude.Left(RfcError.New(e))),
                        Succ: r => r);
                    return res;
                    
                });
        }

        public static Eff<RT,IConnection> Create(IDictionary<string, string> connectionParams, RT runtime)
        {
            return from connectionIO in default(RT).RfcConnectionEff
                from handle in connectionIO.OpenConnection(connectionParams).ToEff(l => l)
                select (IConnection) new Connection<RT, TSettings>(runtime, handle);
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
            var res = default(RT).RfcConnectionEff
                .Bind(io => io.CancelConnection(_connectionHandle).ToEff(l=>l))
                .ToEither(_runtime);
            Dispose();
            return res.ToAsync();
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


        /// <inheritdoc cref="GetAttributes()"/>
        public EitherAsync<RfcError, ConnectionAttributes> GetAttributes()
        {
            return default(RT).RfcConnectionEff
                .Bind(io => io.GetConnectionAttributes(_connectionHandle).ToEff(l => l))
                .ToEither(_runtime).ToAsync();
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
