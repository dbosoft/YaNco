using System.ComponentModel;

namespace Dbosoft.YaNco;

public interface IFunction : IDataContainer
{
    /// <summary>
    /// Direct access to the function handle.
    /// </summary>
    /// <remarks>
    /// Use this property only if you would like to call runtime api methods that are not covered by the <see cref="IFunction"/> interface.
    /// </remarks>
    [Browsable(false)]
    new IFunctionHandle Handle { get; }   
}