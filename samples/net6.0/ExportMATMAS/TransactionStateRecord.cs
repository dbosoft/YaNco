namespace ExportMATMAS;

public class TransactionStateRecord<TData>
{
    public TransactionState State { get; set; }
    public TData? Data { get; set; }
    public string TransactionId { get;  }

    public TransactionStateRecord(string transactionId)
    {
        TransactionId = transactionId;
    }

}

// ReSharper restore InconsistentNaming