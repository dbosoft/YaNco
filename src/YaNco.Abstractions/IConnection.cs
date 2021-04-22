using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IConnection : IDisposable
    {
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait();
        EitherAsync<RfcErrorInfo, Unit> Commit();
        EitherAsync<RfcErrorInfo, Unit> Rollback();
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function);
        EitherAsync<RfcErrorInfo, Unit> AllowStartOfPrograms(StartProgramDelegate callback);
    }
}