using System;
using Dbosoft.YaNco;

namespace Dbosoft.YaNco.Native
{
    public class FunctionDescriptionHandle : IFunctionDescriptionHandle
    {
        internal FunctionDescriptionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        internal IntPtr Ptr { get; }

        public void Dispose()
        {
        }
    }
}