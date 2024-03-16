using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

public readonly struct FunctionInput<RT,TInput> where RT : struct, HasCancel<RT>
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

    public Eff<RT,FunctionProcessed<TOutput>> Process<TOutput>(Func<TInput, Eff<RT,TOutput>> processFunc)
    {
        var function = Function;
        return processFunc(Input).Map(r => new FunctionProcessed<TOutput>(r, function));
    }

    /// <summary>
    /// Async data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/> wrapped in a Task</returns>

    public Aff<RT,FunctionProcessed<TOutput>> Process<TOutput>(Func<TInput, Aff<RT, TOutput>> processFunc)
    {
        var function = Function;
        return processFunc(Input).Map(r => new FunctionProcessed<TOutput>(r, function));
    }

    public Aff<RT, FunctionProcessed<TOutput>> ProcessAsync<TOutput>(Func<TInput, ValueTask<TOutput>> processFunc)
    {
        var function = Function;
        var input = Input;
        return Prelude.Aff( async () => await processFunc(input).ConfigureAwait(false)).Map(r => new FunctionProcessed<TOutput>(r, function));
    }

    public void Deconstruct(out IFunction function, out TInput input)
    {
        function = Function;
        input = Input;
    }
}