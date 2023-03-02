namespace Dbosoft.YaNco
{
    public interface ITransactionalRfcHandler
    {
        RfcRc OnCheck(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnCommit(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnRollback(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnConfirm(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId);
    }
}