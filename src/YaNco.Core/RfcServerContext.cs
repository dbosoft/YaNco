using System;
using Dbosoft.YaNco.Traits;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

internal class RfcServerContext<RT> : IRfcContext<RT> where RT : struct, 
    HasSAPRfc<RT>, HasCancel<RT>
{
    private readonly IRfcServer<RT> _rfcServer;
    private IRfcContext<RT> _currentContext;

    public RfcServerContext(IRfcServer<RT> rfcServer)
    {
        _rfcServer = rfcServer;
    }

    private Eff<RT, IRfcContext<RT>> GetContext()
    {
        _currentContext ??= new RfcContext<RT>( _rfcServer.GetClientConnection());

        return Prelude.SuccessEff(_currentContext);
    }

    public void Dispose()
    {
        _currentContext?.Dispose();
        _currentContext = null;
    }


    public Aff<RT, IFunction> CreateFunction(string name)
    {
        return GetContext().Bind(c => c.CreateFunction(name));
    }

    public Aff<RT, Unit> InvokeFunction(IFunction function)
    {
        return GetContext().Bind(c => c.InvokeFunction(function));
    }

    public Aff<RT, Unit> Ping()
    {
        return GetContext().Bind(c => c.Ping());
    }

    public Aff<RT, Unit> Commit()
    {
        return GetContext().Bind(c => c.Commit());
    }

    public Aff<RT, Unit> CommitAndWait()
    {
        return GetContext().Bind(c => c.CommitAndWait());
    }

    public Aff<RT, Unit> Rollback()
    {
        return GetContext().Bind(c => c.Rollback());
    }

    public Aff<RT, IConnection> GetConnection()
    {
        return GetContext().Bind(c => c.GetConnection());
    }

    // ReSharper disable InconsistentNaming
    public Aff<RT, TResult> CallFunction<TInput, TResult>(string functionName, Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input, Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output)
    {
        return GetContext().Bind(c => c.CallFunction(functionName, Input, Output));
    }

    public Aff<RT, TResult> CallFunction<TResult>(string functionName, Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output)
    {
        return GetContext().Bind(c => c.CallFunction(functionName, Output));
    }

    public Aff<RT, Unit> InvokeFunction(string functionName)
    {
        return GetContext().Bind(c => c.InvokeFunction(functionName));
    }

    public Aff<RT, Unit> InvokeFunction<TInput>(string functionName, Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input)
    {
        return GetContext().Bind(c => c.InvokeFunction(functionName, Input));
    }

    // ReSharper restore InconsistentNaming

}