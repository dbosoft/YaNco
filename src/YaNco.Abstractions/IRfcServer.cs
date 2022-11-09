using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcServer : IDisposable
    {
        bool Disposed { get; }
        IRfcRuntime RfcRuntime { get; }
        EitherAsync<RfcErrorInfo, Unit> Start();
        EitherAsync<RfcErrorInfo, Unit> Stop(int timeout = 0);
        Unit AddConnectionFactory(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionFactory);
        Func<EitherAsync<RfcErrorInfo, IConnection>> ClientConnection { get;  }
    }
}