using LanguageExt;

namespace Dbosoft.YaNco
{
    public delegate EitherAsync<RfcErrorInfo, Unit> RfcFunctionDelegate(IRfcHandle rfcHandle, IFunctionHandle functionHandle);
}