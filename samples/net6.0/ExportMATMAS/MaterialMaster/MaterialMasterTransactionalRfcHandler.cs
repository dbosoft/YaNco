using Dbosoft.YaNco;
using System.Text.Encodings.Web;
using System.Text.Json;
using LanguageExt;
using LanguageExt.Sys;
using LanguageExt.Sys.Traits;

namespace ExportMATMAS.MaterialMaster;


/// <summary>
/// This is a sample implementation of a transactional RFC handler.
/// </summary>
public class MaterialMasterTransactionalRfcHandler<RT> : ITransactionalRfcHandler<RT> 
    where RT : struct, HasConsole<RT>, HasMaterialManager<RT>
{

    public Eff<RT, RfcRc> OnCheck(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Console<RT>.writeLine($"Checking transaction '{transactionId}'")
        from tm in default(RT).MaterialManagerEff
        let result = tm.GetTransaction(transactionId).Match(
            None: () =>
            {
                tm.AddTransaction(transactionId);
                return RfcRc.RFC_OK;
            },
            Some: ta =>
            {
                //check if TA is in valid state, inform sender in case TA is already executed
                return ta.State switch
                {
                    TransactionState.Created => RfcRc.RFC_OK,
                    TransactionState.Executed => RfcRc.RFC_EXECUTED,
                    TransactionState.Committed => RfcRc.RFC_EXECUTED,
                    TransactionState.RolledBack => RfcRc.RFC_OK,
                    _ => throw new ArgumentOutOfRangeException()
                };
            })
        select result;

    public Eff<RT, RfcRc> OnCommit(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Console<RT>.writeLine($"Commit transaction '{transactionId}'")
        from tm in default(RT).MaterialManagerEff
        from ta in tm.GetTransaction(transactionId)
            .ToEff(RfcError.Error($"Transaction {transactionId} not found", RfcRc.RFC_EXTERNAL_FAILURE))
        
        from gNotExecuted in Prelude.guard(ta.State == TransactionState.Executed,
            RfcError.Error($"Transaction {transactionId} not executed", RfcRc.RFC_EXTERNAL_FAILURE).AsError)
        from gHasData in Prelude.guardnot(ta.Data == null, RfcError.Error($"Transaction {transactionId} has no data", RfcRc.RFC_EXTERNAL_FAILURE).AsError)


        // where you would normally commit the transaction, but here we just print the data
        from printData in PrettyPrintMaterial(ta.Data)
        from uPrint in Console<RT>.writeLine(printData)

        from rc in Prelude.Eff(() =>
        {
            ta.State = TransactionState.Committed; 
            return RfcRc.RFC_OK;
        })
        select rc;

    public Eff<RT, RfcRc> OnRollback(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Console<RT>.writeLine($"Rollback transaction '{transactionId}'")
        from tm in default(RT).MaterialManagerEff
        let result = tm.GetTransaction(transactionId).Match(
            None: () => RfcRc.RFC_EXTERNAL_FAILURE,
            Some: ta =>
            {
                ta.State = TransactionState.RolledBack;
                return RfcRc.RFC_OK;
            })
        select result;

    public Eff<RT, RfcRc> OnConfirm(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Console<RT>.writeLine($"Confirm transaction '{transactionId}'")
        from tm in default(RT).MaterialManagerEff
        from result in Prelude.Eff<RT, RfcRc>(_ =>
        {
            // cleanup transaction data
            tm.RemoveTransaction(transactionId);
            return RfcRc.RFC_OK;
        }) select result;

    private static Eff<string> PrettyPrintMaterial(MaterialMasterRecord record)
    {
        return Prelude.Eff( () =>JsonSerializer.Serialize(record, Json.JsonOptions));
    }

}

internal class Json
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        //json don't has to be valid, so disable text encoding for unicode chars
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
}