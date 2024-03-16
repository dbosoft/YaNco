using System;
using System.Collections.Generic;
using Dbosoft.YaNco.Internal;
using LanguageExt;

namespace Dbosoft.YaNco.Live;

public readonly struct LiveSAPRfcServerIO : SAPRfcServerIO
{
    private readonly Option<ILogger> _logger;

    public LiveSAPRfcServerIO(Option<ILogger> logger)
    {
        _logger = logger;
    }


    public Either<RfcError, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams)
    {
        if (connectionParams.Count == 0)
            return new RfcError(new RfcErrorInfo(RfcRc.RFC_EXTERNAL_FAILURE,
                RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, RfcRc.RFC_EXTERNAL_FAILURE.ToString(),
                "Cannot open SAP connection with empty connection settings.",
                "", "", "", "", "", "", ""));

        var loggedParams = new Dictionary<string, string>(connectionParams);

        _logger.IfSome(l => l.LogTrace("creating rfc server", loggedParams));
        IRfcServerHandle handle = Api.CreateServer(connectionParams, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo, true);

    }

    public Either<RfcError, Unit> LaunchServer(IRfcServerHandle rfcServerHandle)
    {
        _logger.IfSome(l => l.LogTrace("starting rfc server", rfcServerHandle));
        var rc = Api.LaunchServer(rfcServerHandle as RfcServerHandle, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout)
    {
        _logger.IfSome(l => l.LogTrace($"stopping rfc server with timeout of {timeout} seconds.", rfcServerHandle));
        var rc = Api.ShutdownServer(rfcServerHandle as RfcServerHandle, timeout, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, RfcServerAttributes> GetServerCallContext(IRfcHandle rfcHandle)
    {
        _logger.IfSome(l => l.LogTrace("reading server context", new { rfcHandle }));
        var rc = Api.GetServerContext(rfcHandle as RfcHandle, out var attributes, out var errorInfo);
        return IOResult.ResultOrError(_logger, attributes, rc, errorInfo);
    }

    public Either<RfcError, IDisposable> AddTransactionHandlers(string sysid,
        Func<IRfcHandle, string, RfcRc> onCheck,
        Func<IRfcHandle, string, RfcRc> onCommit,
        Func<IRfcHandle, string, RfcRc> onRollback,
        Func<IRfcHandle, string, RfcRc> onConfirm)
    {
        var holder = Api.RegisterTransactionFunctionHandlers(sysid,
            onCheck, onCommit, onRollback, onConfirm,
            out var errorInfo);

        return IOResult.ResultOrError(_logger, holder, errorInfo);
    }
}