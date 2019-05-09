using System;
using System.ComponentModel;

namespace Dbosoft.SAP.NWRfc
{
    public interface IFunction : IDataContainer, IDisposable
    {
        [Browsable(false)]
        IFunctionHandle Handle { get; }   
    }
}