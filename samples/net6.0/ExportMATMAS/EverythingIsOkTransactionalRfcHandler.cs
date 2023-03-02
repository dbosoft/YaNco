using Dbosoft.YaNco;

namespace ExportMATMAS;


/// <summary>
/// This is a dummy transactional rfc handler that just confirms everything with Rfc.Ok
/// In a real implementation you have to store and check the transaction id
/// </summary>
public class EverythingIsOkTransactionalRfcHandler : ITransactionalRfcHandler
{
    public RfcRc OnCheck(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Checking transaction '{transactionId}'");
        return RfcRc.RFC_OK;
    }

    public RfcRc OnCommit(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Commit transaction '{transactionId}'");
        return RfcRc.RFC_OK;
    }

    public RfcRc OnRollback(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Rollback transaction '{transactionId}'");

        return RfcRc.RFC_OK;
    }

    public RfcRc OnConfirm(IRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Confirm transaction '{transactionId}'");

        return RfcRc.RFC_OK;
    }
}