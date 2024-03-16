using LanguageExt;

namespace Dbosoft.YaNco;

// ReSharper disable once TypeParameterCanBeVariant
public interface ITransactionalRfcHandler<RT>
    where RT : struct
{
    Eff<RT,RfcRc> OnCheck(IRfcHandle rfcHandle, string transactionId);
    Eff<RT, RfcRc> OnCommit(IRfcHandle rfcHandle, string transactionId);
    Eff<RT, RfcRc> OnRollback(IRfcHandle rfcHandle, string transactionId);
    Eff<RT, RfcRc> OnConfirm(IRfcHandle rfcHandle, string transactionId);
}