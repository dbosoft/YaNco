using Dbosoft.YaNco;
using System.Text.Encodings.Web;
using System.Text.Json;
using Dbosoft.YaNco.Live;

namespace ExportMATMAS;


/// <summary>
/// This is a sample implementation of a transactional RFC handler.
/// </summary>
public class MaterialMasterTransactionalRfcHandler : ITransactionalRfcHandler<SAPRfcRuntime>
{
    private readonly TransactionManager<MaterialMasterRecord> _transactionManager;

    public MaterialMasterTransactionalRfcHandler(TransactionManager<MaterialMasterRecord> transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public RfcRc OnCheck(SAPRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Checking transaction '{transactionId}'");

        try
        {
            return _transactionManager.GetTransaction(transactionId).Match(
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
                });
        }
        catch (Exception)
        {
            // in case of a error it is required to return RFC_EXTERNAL_FAILURE
            return RfcRc.RFC_EXTERNAL_FAILURE;

        }
        
    }

    public RfcRc OnCommit(SAPRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Commit transaction '{transactionId}'");

        // from here on it is save to process the data

        return _transactionManager.GetTransaction(transactionId).Match(
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
            });

    }

    public RfcRc OnRollback(SAPRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Rollback transaction '{transactionId}'");

        return _transactionManager.GetTransaction(transactionId).Match(
            None: () => RfcRc.RFC_EXTERNAL_FAILURE,
            Some: ta =>
            {
                ta.State = TransactionState.RolledBack;
                return RfcRc.RFC_OK;
            });
    }

    public RfcRc OnConfirm(SAPRfcRuntime rfcRuntime, IRfcHandle rfcHandle, string transactionId)
    {
        Console.WriteLine($"Confirm transaction '{transactionId}'");

        // cleanup transaction data
        _transactionManager.RemoveTransaction(transactionId);
        return RfcRc.RFC_OK;
    }

    private static string PrettyPrintMaterial(MaterialMasterRecord record)
    {
        return JsonSerializer.Serialize(record, JsonOptions);
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        //json don't has to be valid, so disable text encoding for unicode chars
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
}