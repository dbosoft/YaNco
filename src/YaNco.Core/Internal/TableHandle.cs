﻿using System;

namespace Dbosoft.YaNco.Internal
{
    public class TableHandle : ITableHandle
    {
        private readonly bool _destroyable;

        internal TableHandle(IntPtr ptr, bool destroyable)
        {
            _destroyable = destroyable;
            Ptr = ptr;
        }

        internal IntPtr Ptr { get; private set; }

        public void Dispose()
        {
            if (Ptr == IntPtr.Zero) return;

            if (_destroyable)
                Interopt.RfcDestroyTable(Ptr, out _);

            Ptr = IntPtr.Zero;
        }
    }
}