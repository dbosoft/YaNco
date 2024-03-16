using System;

namespace Dbosoft.YaNco.Internal;

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