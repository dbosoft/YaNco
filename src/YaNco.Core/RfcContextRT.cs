using System;
using System.Linq;
using System.Threading;
using LanguageExt;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco;

public class RfcContext<RT> : IRfcContext<RT>
    where RT : struct, HasSAPRfcData<RT>, HasCancel<RT>
{
    private readonly Aff<RT, IConnection> _connectionEffect;
    private Option<IConnection> _openedConnection;
    private readonly SemaphoreSlim _semaphore = new(1);

    public RfcContext(Aff<RT, IConnection> connectionEffect)
    {
        _connectionEffect = connectionEffect;
    }

    /// <inheritdoc />
    public Aff<RT, IConnection> GetConnection()
    {
        return from acquire in Prelude.Aff(async () =>
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                return new SemaphoreHolder(_semaphore);
            }).WithRuntime<RT>()
            from used in Prelude.use(acquire, _ =>
            {
                var opened = _openedConnection.Map(c => !c.Disposed).IfNone(false);

                return from conn in opened
                        ? Prelude.SuccessAff(_openedConnection.AsEnumerable().First())
                        : _connectionEffect.Map(c =>
                        {
                            _openedConnection = Prelude.Some(c);
                            return c;
                        })
                    select conn;
            })
            select used;

    }

    /// <inheritdoc />
    public Aff<RT, Unit> InvokeFunction(IFunction function) =>
        from connection in GetConnection()
        from res in  SAPRfc<RT>.invokeFunction(connection, function)
        select res;


    public Aff<RT, Unit> Ping() =>
        from connection in GetConnection()
        from res in SAPRfc<RT>.ping(connection)
        select res;


    public Aff<RT, IFunction> CreateFunction(string name) =>
        from connection in GetConnection()
        from res in  SAPRfc<RT>.createFunction(connection,name)
        select res;

    public Aff<RT, TResult> CallFunction<TInput, TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        from connection in GetConnection()
        from res in SAPRfc<RT>.callFunction(connection, functionName, Input, Output)
        select res;


    public Aff<RT, TResult> CallFunction<TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        from connection in GetConnection()
        from res in SAPRfc<RT>.callFunction(connection, functionName, Output)
        select res;


    public Aff<RT, Unit> InvokeFunction(
        string functionName) =>
        from connection in GetConnection() 
        from res in SAPRfc<RT>.invokeFunction(connection, functionName)
        select res;


    public Aff<RT, Unit> InvokeFunction<TInput>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input) =>
        from connection in GetConnection()
        from res in SAPRfc<RT>.invokeFunction(connection, functionName, Input)
        select res;


    public Aff<RT, Unit> Commit() =>
        from connection in GetConnection()
        from res in SAPRfc<RT>.commit(connection)
        select res;


    public Aff<RT, Unit> CommitAndWait() =>
        from connection in GetConnection() 
        from res in SAPRfc<RT>.commitAndWait(connection) 
        select res;


    public Aff<RT, Unit> Rollback() =>
        from connection in GetConnection() 
        from res in SAPRfc<RT>.rollback(connection) 
        select res;



    protected virtual void Dispose(bool disposing)
    {
        if (!disposing) return;
        _semaphore.Dispose();
        _openedConnection.IfSome(c => c.Dispose());
        _openedConnection = Option<IConnection>.None;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}