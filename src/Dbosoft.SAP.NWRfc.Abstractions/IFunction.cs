using System;
using System.ComponentModel;

namespace Contiva.SAP.NWRfc
{
    public interface IFunction : IDataContainer, IDisposable
    {
        [Browsable(false)]
        IFunctionHandle Handle { get; }   
    }
}