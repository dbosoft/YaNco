using System;
using System.Collections.Generic;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class RfcServer<RT> : IRfcServer<RT>
        where RT : struct, HasSAPRfcServer<RT>, HasSAPRfcLogger<RT>

    {
        private readonly IAgent<AgentMessage, Either<RfcError, object>> _stateAgent;
        public bool Disposed { get; private set; }

        private Aff<RT, IConnection> _connectionEffect;
        private Seq<IDisposable> _references;


        private RfcServer(IRfcServerHandle serverHandle, RT runtime)
        {
            _connectionEffect = Prelude.SuccessAff((IConnection) new ConnectionPlaceholder());

            _stateAgent = Agent.Start<IRfcServerHandle, AgentMessage, Either<RfcError, object>>(
                serverHandle, async (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "", "Rfc Server already destroyed", "", "E", "", "", "", "", "").ToRfcError());
                    var effect = from io in default(RT).RfcServerEff
                        from logger in default(RT).RfcLoggerEff 
                        from processed in Prelude.Aff(async () =>
                        {
                            try
                            {
                                switch (msg)
                                {
                                    case LaunchServerMessage _:
                                    {
                                        var result = await
                                            GetClientConnection().ToEither(runtime)
                                            .Map(c =>
                                            {
                                                c.Dispose();
                                                return Unit.Default;
                                            })
                                            .Bind(_ =>
                                                io.LaunchServer(handle).ToAsync().Map(u => (object)u));
                                        return (handle, result);

                                    }

                                    case ShutdownServerMessage shutdownServerMessage:
                                    {
                                        var result = io.ShutdownServer(
                                            handle, shutdownServerMessage.Timeout).Map(u => (object)u);
                                        return (handle, result);

                                    }

                                    case DisposeMessage disposeMessage:
                                    {
                                        handle.Dispose();

                                        foreach (var disposable in _references)
                                        {
                                            disposable.Dispose();
                                        }

                                        _references = Seq<IDisposable>.Empty;

                                        return (null, Prelude.Left(disposeMessage.ErrorInfo));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.IfSome(l => l.LogException(ex));
                                return (null, Prelude.Left(RfcErrorInfo.Error(ex.Message)));
                            }

                            logger.IfSome(l => l.LogError(
                                $"Invalid rfc server message {msg.GetType()}"));
                            return (null, Prelude.Left(RfcErrorInfo.Error($"Invalid rfc server message {msg.GetType().Name}")));

                        })
                        select processed;
                    var fin  = await effect.Run(runtime);

                    var res = fin.Match(
                        Fail: e => (handle, Prelude.Left(RfcError.New(e))),
                        Succ: r => r);
                    return res;


                });

        }

        public static Eff<RT, IRfcServer<RT>> Create(
            IDictionary<string, string> connectionParams, RT runtime)
        {
            return default(RT).RfcServerEff.Bind(io =>
                io.CreateServer(connectionParams).ToEff(l=>l)
                    .Map(handle => (IRfcServer<RT>)new RfcServer<RT>(handle, runtime)));
        }


        public EitherAsync<RfcError, Unit> Start() =>
            _stateAgent.Tell(new LaunchServerMessage()).ToAsync().Map(_ => Unit.Default);

        public EitherAsync<RfcError, Unit> Stop(int timeout = 0)
            => _stateAgent.Tell(new ShutdownServerMessage(timeout)).ToAsync().Map(_ => Unit.Default);

        public Aff<RT, IConnection> GetClientConnection()
        {
            return _connectionEffect;
        }

        public Unit AddClientConnection(Aff<RT, IConnection> connectionEffect)
        {
            _connectionEffect = connectionEffect;
            return Unit.Default;

        }


        private void ReleaseUnmanagedResources()
        {
            if (Disposed)
                return;

            Disposed = true;
            _stateAgent.Tell(new DisposeMessage(RfcErrorInfo.EmptyResult().ToRfcError()));
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~RfcServer()
        {
            ReleaseUnmanagedResources();
        }

        private class AgentMessage
        {

        }

        private class LaunchServerMessage : AgentMessage
        {

        }

        private class ShutdownServerMessage : AgentMessage
        {
            public int Timeout { get; }

            public ShutdownServerMessage(int timeout)
            {
                Timeout = timeout;
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

        public void AddReferences(IEnumerable<IDisposable> disposables)
        {
            _references = _references.Concat(disposables);
        }


    }
}