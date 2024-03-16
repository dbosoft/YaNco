using System;
using System.ComponentModel;

namespace Dbosoft.YaNco;

public interface IFunction : IDataContainer
{
    [Browsable(false)]
    IFunctionHandle Handle { get; }   
}