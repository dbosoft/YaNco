using System;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public class Function : DataContainer, IDisposable
    {
        internal FunctionHandle Handle;
        //private readonly IRfcRuntime _rfcRuntime;

        internal Function(FunctionHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
        {
            Handle = handle;
            //_rfcRuntime = rfcRuntime;
        }

        public void Dispose()
        {
            Handle?.Dispose();
            Handle = null;
        }

    }
}