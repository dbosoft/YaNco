using System;

namespace Dbosoft.YaNco.Internal;

public class StructureHandle : IStructureHandle, IDataContainerHandle
{
    private readonly bool _destroyable;
    public IntPtr Ptr { get; private set; }

    internal StructureHandle(IntPtr ptr, bool destroyable = false)
    {
        _destroyable = destroyable;
        Ptr = ptr;
    }

    public void Dispose()
    {
        if (Ptr == IntPtr.Zero) return;

        if (_destroyable)
            Interopt.RfcDestroyStructure(Ptr, out _);

        Ptr = IntPtr.Zero;
    }
}