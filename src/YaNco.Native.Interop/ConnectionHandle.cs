using System;

namespace Dbosoft.YaNco.Native
{
    public class ConnectionHandle : IConnectionHandle
    {
        internal IntPtr Ptr { get; }

        internal ConnectionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public void Dispose()
        {
        }
    }
}
