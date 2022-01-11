using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public delegate Either<RfcErrorInfo,Unit> RfcFunctionDelegate(IRfcHandle rfcHandle, IFunctionHandle functionHandle);
}
