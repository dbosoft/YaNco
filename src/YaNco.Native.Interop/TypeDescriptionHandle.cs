using System;

namespace Dbosoft.YaNco.Native
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
            throw new System.NotImplementedException();
        }
    }
}