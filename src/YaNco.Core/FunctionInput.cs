using System;

namespace Dbosoft.YaNco
{
    public readonly struct FunctionInput<TInput>
    {
        public readonly TInput Input;
        public readonly IFunction Function;

        internal FunctionInput(TInput input, IFunction function)
        {
            Input = input;
            Function = function;
        }

        public FunctionProcessed<TOutput> Process<TOutput>(Func<TInput, TOutput> processFunc)
        {
            return new FunctionProcessed<TOutput>(processFunc(Input), Function);
        }

        public void Deconstruct(out IFunction function, out TInput input)
        {
            function = Function;
            input = Input;
        }
    }
}