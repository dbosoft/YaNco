using System;

namespace Dbosoft.YaNco.Native
{
    public interface IDataContainerHandle : Dbosoft.YaNco.IDataContainerHandle
    {
        IntPtr Ptr { get; }
    }
}
