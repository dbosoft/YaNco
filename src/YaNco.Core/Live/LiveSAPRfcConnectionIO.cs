using System.Collections.Generic;
using Dbosoft.YaNco.Internal;
using LanguageExt;

namespace Dbosoft.YaNco.Live;

public readonly struct LiveSAPRfcConnectionIO : SAPRfcConnectionIO
{
    private readonly Option<ILogger> _logger;

    public LiveSAPRfcConnectionIO(Option<ILogger> logger)
    {
        _logger = logger;
    }

    public Either<RfcError, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
    {
        if (connectionParams.Count == 0)
            return new RfcErrorInfo(RfcRc.RFC_EXTERNAL_FAILURE,
                RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, RfcRc.RFC_EXTERNAL_FAILURE.ToString(),
                "Cannot open SAP connection with empty connection settings.",
                "", "", "", "", "", "", "").ToRfcError();

        var loggedParams = new Dictionary<string, string>(connectionParams);

        // ReSharper disable StringLiteralTypo
        if (loggedParams.ContainsKey("passwd"))
            loggedParams["passwd"] = "XXXX";
        // ReSharper restore StringLiteralTypo

        _logger.IfSome(l => l.LogTrace("Opening connection", loggedParams));
        IConnectionHandle handle = Api.OpenConnection(connectionParams, out var errorInfo);
        return IOResult.ResultOrError(_logger, handle, errorInfo, true);
    }


    public Either<RfcError, Unit> CancelConnection(IConnectionHandle connectionHandle)
    {
        _logger.IfSome(l => l.LogTrace("cancelling function", new { connectionHandle }));
        var rc = Api.CancelConnection(connectionHandle as ConnectionHandle, out var errorInfo);
        return IOResult.ResultOrError(_logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle)
    {
        _logger.IfSome(l => l.LogTrace("checking connection state", new { connectionHandle }));
        var rc = Api.IsConnectionHandleValid(connectionHandle as ConnectionHandle, out var isValid, out var errorInfo);
        return IOResult.ResultOrError(_logger, isValid, rc, errorInfo);
    }

    public Either<RfcError, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle)
    {
        _logger.IfSome(l => l.LogTrace("reading connection attributes", new { connectionHandle }));
        var rc = Api.GetConnectionAttributes(connectionHandle as ConnectionHandle, out var attributes, out var errorInfo);
        return IOResult.ResultOrError(_logger, attributes, rc, errorInfo);
    }
}