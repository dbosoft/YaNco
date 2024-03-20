using System;
using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcFunctionIO
{
    Either<RfcError, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle);

    Either<RfcError, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle);

    Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, int index);

    Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, string name);

    Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler);

    Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler);

    Either<RfcError, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle);
    Either<RfcError, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName);
    Either<RfcError, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription);

    Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
        string functionName);

    Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle);
    Either<RfcError, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle);

}