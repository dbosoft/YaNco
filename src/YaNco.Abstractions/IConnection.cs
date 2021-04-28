using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IConnection : IDisposable
    {
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> AllowStartOfPrograms(StartProgramDelegate callback);
        EitherAsync<RfcErrorInfo, Unit> Cancel();

        bool Disposed { get; }

    }
}