namespace Dbosoft.YaNco;

/// <summary>
/// RFC Server state enumeration
/// </summary>
public enum RfcServerState
{
    /// <summary>
    /// Server has been created but not yet started
    /// </summary>
    Initial = 0,

    /// <summary>
    /// Server is starting and registering with gateway
    /// </summary>
    Starting = 1,

    /// <summary>
    /// Server is running and ready to accept RFC calls
    /// </summary>
    Running = 2,

    /// <summary>
    /// Server connection to gateway is broken (will auto-reconnect)
    /// </summary>
    Broken = 3,

    /// <summary>
    /// Server is stopping
    /// </summary>
    Stopping = 4,

    /// <summary>
    /// Server has stopped
    /// </summary>
    Stopped = 5
}

/// <summary>
/// RFC Server state change information
/// </summary>
public readonly struct RfcServerStateChange
{
    /// <summary>
    /// Previous server state
    /// </summary>
    public RfcServerState OldState { get; init; }

    /// <summary>
    /// New server state
    /// </summary>
    public RfcServerState NewState { get; init; }

    public RfcServerStateChange(RfcServerState oldState, RfcServerState newState)
    {
        OldState = oldState;
        NewState = newState;
    }
}
