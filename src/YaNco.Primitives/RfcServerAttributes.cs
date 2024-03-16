using JetBrains.Annotations;

namespace Dbosoft.YaNco;

/// <summary>
/// Attributes of incoming RFC server call
/// </summary>
[PublicAPI]
public class RfcServerAttributes
{
    public readonly string TransactionId;
    public readonly RfcCallType CallType;

    public RfcServerAttributes(string transactionId, RfcCallType callType)
    {
        TransactionId = transactionId;
        CallType = callType;
    }
}


/// <summary>
/// Type of the rfc call
/// </summary>
[PublicAPI]
public enum RfcCallType
{
    /// <summary>
    /// It's a standard synchronous RFC call.
    /// </summary>
    SYNCHRONOUS,

    /// <summary>
    /// This function call is part of a transactional LUW (tRFC).
    /// </summary>
    TRANSACTIONAL,

    /// <summary>
    /// This function call is part of a queued LUW (qRFC).
    /// </summary>
    QUEUED,

    /// <summary>
    /// This function call is part of a background LUW (bgRFC).
    /// </summary>
    BACKGROUND_UNIT
}