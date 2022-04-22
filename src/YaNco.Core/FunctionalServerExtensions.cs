using System;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco
{
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
                    return c.RfcRuntime.AddFunctionHandler(null, func, f => calledFunc(new CalledFunction(f))).ToAsync();
                });


            });

        }

        public static Either<RfcErrorInfo, FunctionProcessed<TOutput>> Process<TInput, TOutput>(
            this Either<RfcErrorInfo, FunctionInput<TInput>> input,
            Func<TInput, TOutput> processFunc)
        {
            return input.Map(i => i.Process(processFunc));
        }

        public static Either<RfcErrorInfo, FunctionProcessed<Unit>> Process<TInput>(
            this Either<RfcErrorInfo, FunctionInput<TInput>> input,
            Action<TInput> processAction)
        {
            return input.Map(i =>
            {
                var (function, input1) = i;
                processAction(input1);
                return new FunctionProcessed<Unit>(Unit.Default, function);
            });
        }

        public static Either<RfcErrorInfo, Unit> Reply<TOutput>(this Either<RfcErrorInfo, FunctionProcessed<TOutput>> self, Func<TOutput, Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, IFunction>> replyFunc)
        {
            return self.Bind(p => p.Reply(replyFunc));
        }

        public static Either<RfcErrorInfo, Unit> NoReply<TOutput>(this Either<RfcErrorInfo, FunctionProcessed<TOutput>> self)
        {
            return self.Bind(p => p.Reply((o, f)=> f));
        }



    }
}
