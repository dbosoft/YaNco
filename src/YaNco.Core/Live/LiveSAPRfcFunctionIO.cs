using System;
using Dbosoft.YaNco.Internal;
using Dbosoft.YaNco.Traits;
using LanguageExt;

namespace Dbosoft.YaNco.Live;

/// <summary>
/// This is the implementation of the SAPRfcFunctionIO interface for live connections.
/// </summary>
public readonly struct LiveSAPRfcFunctionIO : SAPRfcFunctionIO
{
    private readonly Option<ILogger> _logger;
    private readonly SAPRfcDataIO _dataIO;

    public LiveSAPRfcFunctionIO(Option<ILogger> logger, SAPRfcDataIO dataIO)
    {
        _logger = logger;
        _dataIO = dataIO;
    }

    public Either<RfcError, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName)
    {
        _logger.IfSome(l => l.LogTrace("creating function description without connection", functionName));
        IFunctionDescriptionHandle handle = Api.CreateFunctionDescription(functionName, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo);
    }

    public Either<RfcError, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription)
    {
        _logger.IfSome(l => l.LogTrace("adding parameter to function description", new { handle = descriptionHandle, parameter = parameterDescription }));
        var rc = Api.AddFunctionParameter(descriptionHandle as FunctionDescriptionHandle, parameterDescription, out var errorInfo);
        return IOResult.ResultOrError(_logger, descriptionHandle, rc, errorInfo);
    }

    public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
        string functionName)
    {
        _logger.IfSome(l => l.LogTrace("reading function description by function name", functionName));
        IFunctionDescriptionHandle handle = Api.GetFunctionDescription(connectionHandle as ConnectionHandle, functionName, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo);

    }

    public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
    {
        _logger.IfSome(l => l.LogTrace("reading function description by function handle", functionHandle));
        IFunctionDescriptionHandle handle = Api.GetFunctionDescription(functionHandle as FunctionHandle, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo);

    }


    public Either<RfcError, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
    {
        _logger.IfSome(l => l.LogTrace("reading function name by description handle", descriptionHandle));
        var rc = Api.GetFunctionName(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
        return IOResult.ResultOrError(_logger, result, rc, errorInfo);

    }

    public Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
    {
        var io = this;
        var dataIO = _dataIO;
        var holder = Api.RegisterServerFunctionHandler(sysid,
            descriptionHandle as FunctionDescriptionHandle,
            (rfcHandle, functionHandle) =>
            {
                var func = new Function(functionHandle, dataIO, io);
                return handler(rfcHandle, func).MapLeft(l => l.RfcErrorInfo);
            },
            out var errorInfo);

        return IOResult.ResultOrError(_logger, holder, errorInfo);
    }

    public Either<RfcError, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
    {
        _logger.IfSome(l => l.LogTrace("Invoking function", new { connectionHandle, functionHandle }));
        var rc = Api.Invoke(connectionHandle as ConnectionHandle, functionHandle as FunctionHandle, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
    {
        _logger.IfSome(l => l.LogTrace("creating function by function description handle", descriptionHandle));
        IFunctionHandle handle = Api.CreateFunction(descriptionHandle as FunctionDescriptionHandle, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo);

    }

    public Either<RfcError, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
    {
        _logger.IfSome(l => l.LogTrace("reading function parameter count by function description handle", descriptionHandle));
        var rc = Api.GetFunctionParameterCount(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
        return IOResult.ResultOrError(_logger, result, rc, errorInfo);

    }

    public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, int index)
    {
        _logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and index", new { descriptionHandle, index }));
        var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, index, out var result, out var errorInfo);
        return IOResult.ResultOrError(_logger, result, rc, errorInfo);

    }

    public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, string name)
    {
        _logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and name", new { descriptionHandle, name }));
        var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(_logger, result, rc, errorInfo);

    }

    public Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
    {
        var io = this;
        return GetFunctionDescription(function.Handle)
            .Use(used => used.Bind(d => io.AddFunctionHandler(sysid,
                d, handler)));
    }
}