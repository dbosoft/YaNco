using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public interface IRfcContext : IDisposable
    {
        Task<Either<RfcErrorInfo, Function>> CreateFunction(string name);
        Task<Either<RfcErrorInfo, Unit>> InvokeFunction(Function function);
        Task<Either<RfcErrorInfo, IRfcContext>> Ping();
        Task<Either<RfcErrorInfo, Unit>> Commit();
        Task<Either<RfcErrorInfo, Unit>> CommitAndWait();
        Task<Either<RfcErrorInfo, Unit>> Rollback();
    }
}