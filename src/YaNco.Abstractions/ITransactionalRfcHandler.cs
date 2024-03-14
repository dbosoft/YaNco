using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco
{
    public interface ITransactionalRfcHandler<RT>
        where RT : struct
    {
        RfcRc OnCheck(RT runtime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnCommit(RT runtime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnRollback(RT runtime, IRfcHandle rfcHandle, string transactionId);
        RfcRc OnConfirm(RT runtime, IRfcHandle rfcHandle, string transactionId);
    }
}