using System;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    internal class Function : DataContainer, IFunction
    {
        public IFunctionHandle Handle { get; private set; }
        //private readonly IRfcRuntime _rfcRuntime;

        internal Function(IFunctionHandle handle, IRfcRuntime rfcRuntime) : base(handle, rfcRuntime)
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