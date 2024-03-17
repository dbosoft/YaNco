using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.Traits;

public interface SAPRfcServerIO
{
    Either<RfcError, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams);
    Either<RfcError, Unit> LaunchServer(IRfcServerHandle rfcServerHandle);
    Either<RfcError, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout);
    Either<RfcError, RfcServerAttributes> GetServerCallContext(IRfcHandle rfcHandle);

    Either<RfcError, IDisposable> AddTransactionHandlers(string sysid,
        Func<IRfcHandle, string, RfcRc> onCheck,
        Func<IRfcHandle, string, RfcRc> onCommit,
        Func<IRfcHandle, string, RfcRc> onRollback,
        Func<IRfcHandle, string, RfcRc> onConfirm);


}