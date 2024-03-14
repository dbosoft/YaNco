using System.Threading;
using LanguageExt;

namespace Dbosoft.YaNco;

internal class ConnectionPlaceholder : IConnection
{

    private static readonly RfcError ErrorResponse = new RfcErrorInfo(RfcRc.RFC_CLOSED,
        RfcErrorGroup.COMMUNICATION_FAILURE, "",
        "no client connection",
        "", "", "", "", "", "", "").ToRfcError();

    public ConnectionPlaceholder()
    {

    }

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

    public EitherAsync<RfcError, Unit> AllowStartOfPrograms(StartProgramDelegate callback)
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
#pragma warning disable CS0618 // Type or member is obsolete
    public IRfcRuntime RfcRuntime { get; } = new RfcRuntime(SAPRfcRuntime.Default);
#pragma warning restore CS0618 // Type or member is obsolete
    public T GetRuntimeSettings<T>() where T : SAPRfcRuntimeSettings
    {
        return SAPRfcRuntime.Default.Env.Settings as T;
    }
}