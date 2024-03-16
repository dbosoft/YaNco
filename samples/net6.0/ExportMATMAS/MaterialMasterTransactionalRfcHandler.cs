using Dbosoft.YaNco;
using System.Text.Encodings.Web;
using System.Text.Json;
using LanguageExt;

namespace ExportMATMAS;


/// <summary>
/// This is a sample implementation of a transactional RFC handler.
/// </summary>
public class MaterialMasterTransactionalRfcHandler<RT> : ITransactionalRfcHandler<RT> where RT : struct
{
    private readonly TransactionManager<MaterialMasterRecord> _transactionManager;

    public MaterialMasterTransactionalRfcHandler(TransactionManager<MaterialMasterRecord> transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public Eff<RT,RfcRc> OnCheck(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Prelude.Eff(() =>
        {
            Console.WriteLine($"Checking transaction '{transactionId}'");
            return Prelude.unitEff;
        })
        let result = _transactionManager.GetTransaction(transactionId).Match(
            None: () =>
            {
                _transactionManager.AddTransaction(transactionId);
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

    public Eff<RT,RfcRc> OnCommit(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Prelude.Eff(() =>
        {
            Console.WriteLine($"Commit transaction '{transactionId}'");
            return Prelude.unitEff;
        })
        let result = _transactionManager.GetTransaction(transactionId).Match(
            None: () => RfcRc.RFC_EXTERNAL_FAILURE,
            Some: ta =>
            {
                if(ta.State!= TransactionState.Executed)
                    return RfcRc.RFC_EXTERNAL_FAILURE;

                if(ta.Data== null)
                    return RfcRc.RFC_EXTERNAL_FAILURE;

                //simple print the data
                Console.WriteLine(PrettyPrintMaterial(ta.Data));
                ta.State = TransactionState.Committed;
                return RfcRc.RFC_OK;
            })
        select result;

    public Eff<RT, RfcRc> OnRollback(IRfcHandle rfcHandle, string transactionId) =>
        from uLog in Prelude.Eff(() =>
        {
            Console.WriteLine($"Rollback transaction '{transactionId}'");
            return Prelude.unitEff;
        })
        let result =  _transactionManager.GetTransaction(transactionId).Match(
            None: () => RfcRc.RFC_EXTERNAL_FAILURE,
            Some: ta =>
            {
                ta.State = TransactionState.RolledBack;
                return RfcRc.RFC_OK;
            })
        select result;

    public Eff<RT,RfcRc> OnConfirm(IRfcHandle rfcHandle, string transactionId)
    {
        return Prelude.Eff<RT,RfcRc>(_ =>
        {
            Console.WriteLine($"Confirm transaction '{transactionId}'");

            // cleanup transaction data
            _transactionManager.RemoveTransaction(transactionId);
            return RfcRc.RFC_OK;
        });
    }

    private static string PrettyPrintMaterial(MaterialMasterRecord record)
    {
        return JsonSerializer.Serialize(record, Json.JsonOptions);
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