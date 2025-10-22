using System;
using System.Runtime.InteropServices;

namespace Dbosoft.YaNco.Internal;

/// <summary>
/// Holds references to server event listener callbacks while they are registered.
/// Prevents GC from collecting delegates that are in use by unmanaged RFC library.
/// </summary>
internal class ServerEventListeners : IDisposable
{
    // CRITICAL: Hold delegate references to prevent GC collection
    public readonly Interopt.ServerStateChangedCallback StateCallback;
    public readonly Interopt.ServerErrorCallback ErrorCallback;
    private readonly IntPtr _serverHandle;

    public ServerEventListeners(
        IntPtr serverHandle,
        Action<RfcServerStateChange> onStateChange,
        Action<ConnectionAttributes, RfcErrorInfo> onError)
    {
        _serverHandle = serverHandle;

        // Wrap managed callbacks in unmanaged delegates
        // These execute on RFC library worker threads!
        if (onStateChange != null)
        {
            StateCallback = (IntPtr _, ref Interopt.RfcServerStateChange change) =>
            {
                try
                {
                    // Map internal enum to public enum
                    var publicChange = new RfcServerStateChange(
                        (RfcServerState)(int)change.OldState,
                        (RfcServerState)(int)change.NewState
                    );
                    onStateChange(publicChange);
                }
                catch
                {
                    // Never throw from native callback - would corrupt unmanaged state
                }
            };
        }

        if (onError != null)
        {
            ErrorCallback = (IntPtr _, IntPtr clientInfoPtr, ref RfcErrorInfo err) =>
            {
                try
                {
                    // clientInfoPtr can be null if no client connected during error
                    ConnectionAttributes clientInfo = null;
                    if (clientInfoPtr != IntPtr.Zero)
                    {
                        var attrs = Marshal.PtrToStructure<Interopt.RfcAttributes>(clientInfoPtr);
                        clientInfo = attrs.ToConnectionAttributes();
                    }
                    onError(clientInfo, err);
                }
                catch
                {
                    // Never throw from native callback - would corrupt unmanaged state
                }
            };
        }
    }

    private void ReleaseUnmanagedResources()
    {
        // Unregister by passing null to RFC APIs
        if (StateCallback != null)
            Interopt.RfcAddServerStateChangedListener(_serverHandle, null, out _);
        if (ErrorCallback != null)
            Interopt.RfcAddServerErrorListener(_serverHandle, null, out _);
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~ServerEventListeners()
    {
        ReleaseUnmanagedResources();
    }
}
