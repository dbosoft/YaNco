﻿using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IConnection : IDisposable
    {
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait();
        EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> Commit();
        EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> Rollback();
        EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function);
        EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken);
        EitherAsync<RfcErrorInfo, Unit> AllowStartOfPrograms(StartProgramDelegate callback);
        EitherAsync<RfcErrorInfo, Unit> Cancel();

        bool Disposed { get; }
        IRfcRuntime RfcRuntime { get;  }

    }
}