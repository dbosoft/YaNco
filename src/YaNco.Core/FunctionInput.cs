using System;
using System.Threading.Tasks;

namespace Dbosoft.YaNco;

public readonly struct FunctionInput<TInput>
{
    public readonly TInput Input;
    public readonly IFunction Function;

    internal FunctionInput(TInput input, IFunction function)
    {
        Input = input;
        Function = function;
    }

    /// <summary>
    /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/></returns>

    public FunctionProcessed<TOutput> Process<TOutput>(Func<TInput, TOutput> processFunc)
    {
        return new FunctionProcessed<TOutput>(processFunc(Input), Function);
    }

    /// <summary>
    /// Async data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/> wrapped in a Task</returns>

    public async Task<FunctionProcessed<TOutput>> ProcessAsync<TOutput>(Func<TInput, Task<TOutput>> processFunc)
    {
        return new FunctionProcessed<TOutput>(await processFunc(Input), Function);
    }

    public void Deconstruct(out IFunction function, out TInput input)
    {
        function = Function;
        input = Input;
    }
}