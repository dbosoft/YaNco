using System;

namespace Dbosoft.YaNco.Internal
{
    public class ConnectionHandle : RfcHandle, IConnectionHandle
    {

        internal ConnectionHandle(IntPtr ptr) : base(ptr)
        {
        }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero) return;

            Interopt.RfcCloseConnection(Ptr, out _);
            
            Ptr = IntPtr.Zero;
        }
    }
}
