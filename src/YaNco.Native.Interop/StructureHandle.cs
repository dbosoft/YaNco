using System;

namespace Dbosoft.YaNco.Native
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
            throw new System.NotImplementedException();
        }
    }
}