using System;
using System.Collections.Generic;
using Dbosoft.Functional;
using Dbosoft.YaNco.Traits;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

public class RfcServer<RT> : IRfcServer<RT>
    where RT : struct, HasSAPRfcServer<RT>, HasSAPRfc<RT>, HasCancel<RT>

{
    private readonly IAgent<AgentMessage, Either<RfcError, object>> _stateAgent;
    private readonly IRfcServerHandle _serverHandle;
    public bool Disposed { get; private set; }

    private Aff<RT, IConnection> _connectionEffect;
    private Seq<IDisposable> _references;


    private RfcServer(IRfcServerHandle serverHandle, RT runtime)
    {
        _serverHandle = serverHandle;
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
                    from processed in Unit.Default.Apply(_ =>
                    {
                        try
                        {
                            switch (msg)
                            {
                                case LaunchServerMessage:
                                {
                                    var result = GetClientConnection()
                                        .Map(c =>
                                        {
                                            c.Dispose();
                                            return Unit.Default;
                                        })
                                        .Bind(_ =>
                                            io.LaunchServer(handle).ToEff(l=>l).Map(u => (object)u));
                                    return result;

                                }

                                case ShutdownServerMessage shutdownServerMessage:
                                {
                                    var result = io.ShutdownServer(
                                        handle, shutdownServerMessage.Timeout).Map(u => (object)u).ToEff(l => l);
                                    return result;

                                }

                                case DisposeMessage disposeMessage:
                                {
                                    handle.Dispose();

                                    foreach (var disposable in _references)
                                    {
                                        disposable.Dispose();
                                    }

                                    _references = Seq<IDisposable>.Empty;
                                    handle = null;
                                    return Prelude.FailEff<object>(disposeMessage.ErrorInfo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.IfSome(l => l.LogException(ex));
                            return Prelude.FailEff<object>(RfcError.Error(ex.Message));
                        }

                        logger.IfSome(l => l.LogError(
                            $"Invalid rfc server message {msg.GetType()}"));
                        return Prelude.FailEff<object>(RfcError.Error($"Invalid rfc server message {msg.GetType().Name}"));

                    })
                    select processed;

                var res = await effect.ToEither(runtime);
                return (handle,res);


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

    /// <summary>
    /// Gets the RFC server handle for internal use
    /// </summary>
    internal IRfcServerHandle ServerHandle => _serverHandle;


}