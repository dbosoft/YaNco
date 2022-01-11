using System;

namespace Dbosoft.YaNco.Internal
{
    public class RfcHandle : IRfcHandle
    {
        internal IntPtr Ptr { get; set; }

        internal RfcHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }
    }
}