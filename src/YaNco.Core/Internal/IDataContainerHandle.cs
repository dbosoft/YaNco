using System;

namespace Dbosoft.YaNco.Internal
{
    public interface IDataContainerHandle : Dbosoft.YaNco.IDataContainerHandle
    {
        IntPtr Ptr { get; }
    }
}
