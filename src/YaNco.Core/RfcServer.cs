using System;
using System.Collections.Generic;
using Dbosoft.Functional;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public class RfcServer : IRfcServer
    {
        public IRfcRuntime RfcRuntime { get; }
        private readonly IAgent<AgentMessage, Either<RfcErrorInfo, object>> _stateAgent;
        public bool Disposed { get; private set; }

        private RfcServer(IRfcServerHandle serverHandle, IRfcRuntime rfcRuntime)
        {
            RfcRuntime = rfcRuntime;

            _stateAgent = Agent.Start<IRfcServerHandle, AgentMessage, Either<RfcErrorInfo, object>>(
                serverHandle, (handle, msg) =>
                {
                    if (handle == null)
                        return (null,
                            new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE,
                                "", "Rfc Server already destroyed", "", "E", "", "", "", "", ""));

                    try
                    {
                        switch (msg)
                        {
                            case LaunchServerMessage _:
                            {
                                var result = rfcRuntime.LaunchServer(handle);
                                return (handle, result);

                            }

                            case ShutdownServerMessage shutdownServerMessage:
                            {
                                var result = rfcRuntime.ShutdownServer(
                                    handle, shutdownServerMessage.Timeout);
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
                        rfcRuntime.Logger.IfSome(l => l.LogException(ex));
                    }

                    throw new InvalidOperationException();
                });

        }

        public static EitherAsync<RfcErrorInfo, IRfcServer> Create(
            IDictionary<string, string> connectionParams, IRfcRuntime runtime)
        {
            return runtime.CreateServer(connectionParams).ToAsync().Map(handle => (IRfcServer)new RfcServer(handle, runtime));
        }


        public EitherAsync<RfcErrorInfo, Unit> Start()
            => _stateAgent.Tell(new LaunchServerMessage()).ToAsync().Map(_ => Unit.Default);

        public EitherAsync<RfcErrorInfo, Unit> Stop(int timeout = 0)
            => _stateAgent.Tell(new ShutdownServerMessage(timeout)).ToAsync().Map(_ => Unit.Default);

        private void ReleaseUnmanagedResources()
        {
            if (Disposed)
                return;

            Disposed = true;
            _stateAgent.Tell(new DisposeMessage(RfcErrorInfo.EmptyResult()));
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
            public readonly RfcErrorInfo ErrorInfo;

            public DisposeMessage(RfcErrorInfo errorInfo)
            {
                ErrorInfo = errorInfo;
            }
        }

    }
}