using System;

namespace Dbosoft.YaNco.Internal;

/// <summary>
/// Used to hold the references of the transaction handler callbacks while
/// they are registered.
/// </summary>
internal class TransactionEventHandlers : IDisposable
{

    public readonly Interopt.TransactionEventCallback OnCheck;
    public readonly Interopt.TransactionEventCallback OnCommit;
    public readonly Interopt.TransactionEventCallback OnRollback;
    public readonly Interopt.TransactionEventCallback OnConfirm;
    public readonly string Sysid;

    public TransactionEventHandlers(string sysid,
        Func<IRfcHandle, string, RfcRc> onCheck,
        Func<IRfcHandle, string, RfcRc> onCommit,
        Func<IRfcHandle, string, RfcRc> onRollback,
        Func<IRfcHandle, string, RfcRc> onConfirm
    )
    {
        Sysid = sysid;
        OnCheck = (rfcHandle, tid) => onCheck(new RfcHandle(rfcHandle), tid);
        OnCommit = (rfcHandle, tid) => onCommit(new RfcHandle(rfcHandle), tid);
        OnRollback = (rfcHandle, tid) => onRollback(new RfcHandle(rfcHandle), tid);
        OnConfirm = (rfcHandle, tid) => onConfirm(new RfcHandle(rfcHandle), tid);
    }

    private void ReleaseUnmanagedResources()
    {
        Interopt.RfcInstallTransactionHandlers(Sysid,
            null, null, null, null, out _);

    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~TransactionEventHandlers()
    {
        ReleaseUnmanagedResources();
    }
}