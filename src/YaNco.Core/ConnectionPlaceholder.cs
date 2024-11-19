using System;
using System.Threading;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco;

internal class ConnectionPlaceholder : IConnection
{

    private static readonly RfcError ErrorResponse = new RfcErrorInfo(RfcRc.RFC_CLOSED,
        RfcErrorGroup.COMMUNICATION_FAILURE, "",
        "no client connection",
        "", "", "", "", "", "", "").ToRfcError();

    public void Dispose()
    {
        Disposed = true;
    }

    public EitherAsync<RfcError, Unit> CommitAndWait()
    {
        return ErrorResponse;

    }

    public EitherAsync<RfcError, Unit> CommitAndWait(CancellationToken cancellationToken)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> Commit()
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> Commit(CancellationToken cancellationToken)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> Rollback()
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> Rollback(CancellationToken cancellationToken)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, IStructure> CreateStructure(string name)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, IFunction> CreateFunction(string name)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function)
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken)
    {
        return ErrorResponse;
    }


    public EitherAsync<RfcError, Unit> Cancel()
    {
        return ErrorResponse;
    }

    public EitherAsync<RfcError, ConnectionAttributes> GetAttributes()
    {
        return ErrorResponse;
    }

    public bool Disposed { get; private set; }

    [Obsolete(Deprecations.RfcRuntime)]
    public IRfcRuntime RfcRuntime { get; } = new RfcRuntime(SAPRfcRuntime.Default);
    public IHasEnvRuntimeSettings ConnectionRuntime { get; } = SAPRfcRuntime.Default;
    public IConnectionHandle Handle { get; }
}