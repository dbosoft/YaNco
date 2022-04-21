using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco
{

    public readonly struct CalledFunction
    {
        public readonly IFunction Function;

        internal CalledFunction(IFunction function)
        {
            Function = function;
        }

        public FunctionProcessed<TOutput> Process<TOutput>(Func<TOutput> processFunc)
        {
            return new FunctionProcessed<TOutput>(processFunc(), Function);
        }

        public Either<RfcErrorInfo,FunctionInput<TInput>> Input<TInput>(Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> inputFunc)
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
        private readonly TOutput Output;
        private readonly IFunction Function;

        internal FunctionProcessed(TOutput output, IFunction function)
        {
            Output = output;
            Function = function;
        }

        public Either<RfcErrorInfo,Unit> Reply(Func<TOutput, Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, IFunction>> replyFunc)
        {
            return replyFunc(Output, Prelude.Right(Function)).Map(_=>Unit.Default);
        }
    }

    public static class FunctionalServerExtensions
    {
        /// <summary>
        /// CallFunction with input and output with RfcErrorInfo lifted input and output functions.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="functionName">ABAP function name</param>
        /// <param name="calledFunc">callback for function</param>
        /// <returns></returns>
        public static EitherAsync<RfcErrorInfo, Unit> OnFunctionCalled(
            this IRfcContext context, 
            string functionName,
            Func<CalledFunction, Either<RfcErrorInfo,Unit>> calledFunc)
        {

            return context.GetConnection().Bind(c =>
            {
                return c.CreateFunction(functionName).Bind(func =>
                {
                    return c.RfcRuntime.AddFunctionHandler(null, func, f => calledFunc(new CalledFunction(func))).ToAsync();
                });


            });

        }


        //public static EitherAsync<RfcErrorInfo, Unit> OnFunctionCalled<TInput>(
        //    this IRfcContext context,
        //    string functionName,
        //    Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> Input,
        //    Action<TInput> Process)
        //{
        //    EitherAsync<RfcErrorInfo, Unit> CallProcess(TInput input)
        //    {
        //        Process(input);
        //        return Unit.Default;
        //    }


        //    return context.CreateFunction(functionName).Use(
        //        ef => ef.Bind(func =>

        //            from input in Input(Prelude.Right(func)).ToAsync()
        //            from _ in CallProcess(input)
        //            select Unit.Default)
        //    );

        //}

        //public static EitherAsync<RfcErrorInfo, Unit> OnFunctionCalled<TInput>(
        //    this IRfcContext context,
        //    string functionName,
        //    Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> Input,
        //    Func<TInput, Task> Process)
        //{
        //    async Task<Either<RfcErrorInfo, Unit>> CallProcess(TInput input)
        //    {
        //        await Process(input);
        //        return Unit.Default;
        //    }


        //    return context.CreateFunction(functionName).Use(
        //        ef => ef.Bind(func =>

        //            from input in Input(Prelude.Right(func)).ToAsync()
        //            from _ in CallProcess(input).ToAsync()
        //            select Unit.Default)
        //    );

        //}

        ///// <summary>
        ///// CallFunction with RfcErrorInfo lifted output.
        ///// </summary>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="context"></param>
        ///// <param name="functionName"></param>
        ///// <param name="Output">Output function lifted in either monad.</param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public static EitherAsync<RfcErrorInfo, TResult> OnFunctionCalled<TResult>(
        //    this IRfcContext context, 
        //    string functionName, 
        //    Func<Either<RfcErrorInfo,IFunction>, Either<RfcErrorInfo, TResult>> Output,
        //    CancellationToken cancellationToken = default)
        //{
        //    return context.CreateFunction(functionName).Use(
        //        ef => ef.Bind(func =>
        //            from _ in context.InvokeFunction(func, cancellationToken)
        //            from output in Output(Prelude.Right(func)).ToAsync()
        //            select output)
        //    );
        //}

    }
}
