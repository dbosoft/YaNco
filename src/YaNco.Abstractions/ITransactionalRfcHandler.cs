namespace Dbosoft.YaNco;

// ReSharper disable once TypeParameterCanBeVariant
public interface ITransactionalRfcHandler<RT>
    where RT : struct
{
    RfcRc OnCheck(RT runtime, IRfcHandle rfcHandle, string transactionId);
    RfcRc OnCommit(RT runtime, IRfcHandle rfcHandle, string transactionId);
    RfcRc OnRollback(RT runtime, IRfcHandle rfcHandle, string transactionId);
    RfcRc OnConfirm(RT runtime, IRfcHandle rfcHandle, string transactionId);
}