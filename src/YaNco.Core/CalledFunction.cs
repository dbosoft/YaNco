using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public readonly struct CalledFunction
    {
        public readonly IFunction Function;
        private readonly Func<IRfcContext> _rfcContextFunc;

        internal CalledFunction(IFunction function, Func<IRfcContext> rfcContextFunc)
        {
            Function = function;
            _rfcContextFunc = rfcContextFunc;
        }

        public Either<RfcErrorInfo, FunctionInput<TInput>> Input<TInput>(Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> inputFunc)
        {
            var function = Function;
            return inputFunc(Prelude.Right(function)).Map(input => new FunctionInput<TInput>(input, function));
        }

        public T UseRfcContext<T>(Func<IRfcContext, T> mapFunc)
        {
            using (var rfcContext = _rfcContextFunc())
            {
                return mapFunc(rfcContext);
            }
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