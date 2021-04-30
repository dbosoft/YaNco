using System;

namespace Dbosoft.YaNco.Internal
{
    public class ConnectionHandle : IConnectionHandle
    {
        internal IntPtr Ptr { get; private set; }

        internal ConnectionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero) return;

            Api.RemoveCallbackHandler(Ptr);
            Interopt.RfcCloseConnection(Ptr, out _);
            
            Ptr = IntPtr.Zero;
        }
    }
}
