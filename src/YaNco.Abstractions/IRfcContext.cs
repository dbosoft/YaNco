using System;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcContext : IDisposable
    {
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function);
        EitherAsync<RfcErrorInfo, IRfcContext> Ping();
        EitherAsync<RfcErrorInfo, Unit> Commit();
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait();
        EitherAsync<RfcErrorInfo, Unit> Rollback();

        Task<Either<RfcErrorInfo, IRfcContext>> PingAsync();
        Task<Either<RfcErrorInfo, Unit>> CommitAsync();
        Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync();
        Task<Either<RfcErrorInfo, Unit>> RollbackAsync();
    }
}