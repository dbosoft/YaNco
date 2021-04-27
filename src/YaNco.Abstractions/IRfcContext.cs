using System;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcContext : IDisposable
    {
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name, CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, IRfcContext> Ping(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken = default);
        EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken = default);

        Task<Either<RfcErrorInfo, IRfcContext>> PingAsync(CancellationToken cancellationToken = default);
        Task<Either<RfcErrorInfo, Unit>> CommitAsync(CancellationToken cancellationToken = default);
        Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken = default);
        Task<Either<RfcErrorInfo, Unit>> RollbackAsync(CancellationToken cancellationToken = default);
    }
}