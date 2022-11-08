using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public readonly struct CalledFunction
    {
        public readonly IFunction Function;

        internal CalledFunction(IFunction function)
        {
            Function = function;
        }

        public Either<RfcErrorInfo, FunctionInput<TInput>> Input<TInput>(Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> inputFunc)
        {
            var function = Function;
            return inputFunc(Prelude.Right(function)).Map(input => new FunctionInput<TInput>(input, function));
        }


    }

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

    public readonly struct FunctionProcessed<TOutput>
    {
        private readonly TOutput _output;
        private readonly IFunction _function;

        internal FunctionProcessed(TOutput output, IFunction function)
        {
            _output = output;
            _function = function;
        }

        public Either<RfcErrorInfo, Unit> Reply(Func<TOutput, Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, IFunction>> replyFunc)
        {
            return replyFunc(_output, Prelude.Right(_function)).Map(_ => Unit.Default);
        }
    }
}