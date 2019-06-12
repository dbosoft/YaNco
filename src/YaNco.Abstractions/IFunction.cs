using System;
using System.ComponentModel;

namespace Dbosoft.YaNco
{
    public interface IFunction : IDataContainer, IDisposable
    {
        [Browsable(false)]
        IFunctionHandle Handle { get; }   
    }
}