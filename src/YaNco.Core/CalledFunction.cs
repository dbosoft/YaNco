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

        /// <summary>
        /// Input processing for a called function. Use this method to extract values from the rfc function.
        /// </summary>
        /// <typeparam name="TInput">Type of data extracted from function. Could be any type.</typeparam>
        /// <param name="inputFunc">Function to map from RFC function to the desired input type</param>
        /// <returns><see cref="FunctionInput{TInput}"/> wrapped in a <see cref="Either{L,R}"/> </returns>
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