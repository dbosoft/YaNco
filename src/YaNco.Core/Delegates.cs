using Dbosoft.YaNco;
using LanguageExt;

public delegate EitherAsync<RfcErrorInfo, Unit> RfcFunctionDelegate(IRfcHandle rfcHandle, IFunctionHandle functionHandle);