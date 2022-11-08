using Dbosoft.YaNco;
using LanguageExt;

public delegate Either<RfcErrorInfo, Unit> RfcFunctionDelegate(IRfcHandle rfcHandle, IFunctionHandle functionHandle);