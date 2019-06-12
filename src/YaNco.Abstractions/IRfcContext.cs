using System;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcContext : IDisposable
    {
        Task<Either<RfcErrorInfo, IFunction>> CreateFunction(string name);
        Task<Either<RfcErrorInfo, Unit>> InvokeFunction(IFunction function);
        Task<Either<RfcErrorInfo, IRfcContext>> Ping();
        Task<Either<RfcErrorInfo, Unit>> Commit();
        Task<Either<RfcErrorInfo, Unit>> CommitAndWait();
        Task<Either<RfcErrorInfo, Unit>> Rollback();
    }
}