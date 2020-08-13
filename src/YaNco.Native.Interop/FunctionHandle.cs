using System;

namespace Dbosoft.YaNco.Native
{
    public class FunctionHandle : IFunctionHandle, IDataContainerHandle
    {
        internal FunctionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public IntPtr Ptr { get; }

        public void Dispose()
        {
        }
    }
}