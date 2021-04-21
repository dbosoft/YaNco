using System;

namespace Dbosoft.YaNco.Internal
{
    public class TypeDescriptionHandle : ITypeDescriptionHandle
    {
        internal TypeDescriptionHandle(IntPtr ptr)
        {
            Ptr = ptr;
        }

        internal IntPtr Ptr { get; }

        public void Dispose()
        {
        }
    }
}