using LanguageExt;

namespace Dbosoft.YaNco.Live;

public static class IOResult
{
    public static Either<RfcError, TResult> ResultOrError<TResult>(
        Option<ILogger> logger,
        TResult result, RfcErrorInfo errorInfo, bool logAsError = false)
    {
        if (result == null || errorInfo.Code != RfcRc.RFC_OK)
        {
            logger.IfSome(l =>
            {
                if (errorInfo.Code == RfcRc.RFC_OK)
                {
                    if (logAsError)
                        l.LogError("received null result from api call.");
                    else
                        l.LogDebug("received null result from api call.");
                }

                if (logAsError)
                    l.LogError("received error from rfc call", errorInfo);
                else
                    l.LogDebug("received error from rfc call", errorInfo);
            });
            return errorInfo.ToRfcError();
        }

        logger.IfSome(l => l.LogTrace("received result value from rfc call", result));

        return result;
    }

    public static Either<RfcError, TResult> ResultOrError<TResult>(
        Option<ILogger> logger,
        TResult result, RfcRc rc, RfcErrorInfo errorInfo)
    {
        if (rc != RfcRc.RFC_OK)
        {
            logger.IfSome(l => l.LogDebug("received error from rfc call", errorInfo));
            return errorInfo.ToRfcError();
        }

        logger.IfSome(l => l.LogTrace("received result value from rfc call", result));
        return result;
    }
}