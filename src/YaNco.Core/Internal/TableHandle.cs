using System;

namespace Dbosoft.YaNco.Internal
{
    public class TableHandle : ITableHandle
    {
        internal TableHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        internal IntPtr Ptr { get; }

        public void Dispose()
        {
        }
    }
}