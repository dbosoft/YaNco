using System;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcContext : IDisposable
    {
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name, CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, IRfcContext> Ping();
        EitherAsync<RfcErrorInfo, IRfcContext> Ping(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> Commit();
        EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait();
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> Rollback();
        EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken);

        Task<Either<RfcErrorInfo, IRfcContext>> PingAsync();
        Task<Either<RfcErrorInfo, IRfcContext>> PingAsync(CancellationToken cancellationToken);
        Task<Either<RfcErrorInfo, Unit>> CommitAsync();
        Task<Either<RfcErrorInfo, Unit>> CommitAsync(CancellationToken cancellationToken);
        Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync();
        Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken);
        Task<Either<RfcErrorInfo, Unit>> RollbackAsync();
        Task<Either<RfcErrorInfo, Unit>> RollbackAsync(CancellationToken cancellationToken);
    }
}