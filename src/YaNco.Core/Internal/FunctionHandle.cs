using System;

namespace Dbosoft.YaNco.Internal
{
    public class FunctionHandle : IFunctionHandle, IDataContainerHandle
    {
        internal FunctionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        public IntPtr Ptr { get; private set; }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero) return;

            Interopt.RfcDestroyFunction(Ptr, out _);
            Ptr = IntPtr.Zero;
        }
    }
}