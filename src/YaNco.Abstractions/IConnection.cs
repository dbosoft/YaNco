using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IConnection : IDisposable
    {
        Task<Either<RfcErrorInfo, Unit>> CommitAndWait();
        Task<Either<RfcErrorInfo, Unit>> Commit();
        Task<Either<RfcErrorInfo, Unit>> Rollback();
        Task<Either<RfcErrorInfo, IFunction>> CreateFunction(string name);
        Task<Either<RfcErrorInfo, Unit>> InvokeFunction(IFunction function);
        Task<Either<RfcErrorInfo, Unit>> AllowStartOfPrograms(StartProgramDelegate callback);
    }
}