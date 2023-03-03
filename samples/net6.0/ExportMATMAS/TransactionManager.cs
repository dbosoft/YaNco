using LanguageExt;

namespace ExportMATMAS;

public class TransactionManager<TData>
{
    private HashMap<string, TransactionStateRecord<TData>> _transactions;

    public Option<TransactionStateRecord<TData>> GetTransaction(string transactionId)
    {
        return _transactions.Find(transactionId);
    }

    public TransactionStateRecord<TData> AddTransaction(string transactionId)
    {
        var record = new TransactionStateRecord<TData>(transactionId);
        _transactions = _transactions.Add(transactionId,record);
        return record;
    }

    public Unit RemoveTransaction(string transactionId)
    {
        _transactions = _transactions.Remove(transactionId);
        return Unit.Default;
    }
}