using System;

namespace Dbosoft.YaNco.Internal
{
    public class StructureHandle : IStructureHandle, IDataContainerHandle
    {
        public IntPtr Ptr { get; }

        internal StructureHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public void Dispose()
        {
        }
    }

}