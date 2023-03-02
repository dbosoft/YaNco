using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcServer : IDisposable
    {
        bool Disposed { get; }
        IRfcRuntime RfcRuntime { get; }
        EitherAsync<RfcErrorInfo, Unit> Start();
        EitherAsync<RfcErrorInfo, Unit> Stop(int timeout = 0);
        EitherAsync<RfcErrorInfo, IConnection> OpenClientConnection();

        Unit AddConnectionFactory(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionFactory);

        /// <summary>
        /// Adds references to the server that should be disposed when the server is disposed.
        /// </summary>
        /// <param name="disposables"></param>
        void AddReferences(IEnumerable<IDisposable> disposables);


    }
}