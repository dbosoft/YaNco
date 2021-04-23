using System;
using System.Linq;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public static class FunctionalFunctionsExtensions
    {

        internal static EitherAsync<RfcErrorInfo, R1> Commit<R1>(this EitherAsync<RfcErrorInfo, R1> self,
            IRfcContext context)
        {
            return self.Bind(res => context.Commit().Map(u => res));
        }

        internal static EitherAsync<RfcErrorInfo, R1> CommitAndWait<R1>(this EitherAsync<RfcErrorInfo, R1> self,
            IRfcContext context)
        {
            return self.Bind(res => context.CommitAndWait().Map(u => res));
        }

        public static EitherAsync<RfcErrorInfo, IFunction> HandleReturn(this EitherAsync<RfcErrorInfo, IFunction> self)
        {
            return self.ToEither().Map(f => f.HandleReturn()).ToAsync();
        }

        public static EitherAsync<RfcErrorInfo, Unit> AsUnit(this EitherAsync<RfcErrorInfo, IFunction> self)
        {
            return self.Map(_ => Unit.Default);
        }

        public static Either<RfcErrorInfo, IFunction> HandleReturn(this Either<RfcErrorInfo, IFunction> self)
        {
            return self.Bind(f => (
                from ret in f.GetStructure("RETURN")
                from type in ret.GetField<string>("TYPE")
                from id in ret.GetField<string>("ID")
                from number in ret.GetField<string>("NUMBER")
                from message in ret.GetField<string>("MESSAGE")
                from v1 in ret.GetField<string>("MESSAGE_V1")
                from v2 in ret.GetField<string>("MESSAGE_V2")
                from v3 in ret.GetField<string>("MESSAGE_V3")
                from v4 in ret.GetField<string>("MESSAGE_V4")

                from _ in ErrorOrResult(f, type, id, number, message, v1, v2, v3, v4)
                select f));

        }

        private static Either<RfcErrorInfo, TResult> ErrorOrResult<TResult>(TResult result, string type, string id, string number, string message, string v1, string v2, string v3, string v4)
        {
            if (type.Contains('E') || type.Contains('A'))
                return new RfcErrorInfo(RfcRc.RFC_ABAP_MESSAGE, RfcErrorGroup.ABAP_APPLICATION_FAILURE, "", message, id, type, number, v1, v2, v3, v4);

            return result;
        }

        // ReSharper disable InconsistentNaming

        /// <summary>
        /// CallFunction with input and output with RfcErrorInfo lifted input and output functions.
        /// </summary>
        /// <typeparam name="TRInput"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"></param>
        /// <param name="functionName"></param>
        /// <param name="Input">Input function lifted in either monad.</param>
        /// <param name="Output">Output function lifted in either monad.</param>
        /// <returns></returns>
        public static EitherAsync<RfcErrorInfo, TResult> CallFunction<TRInput, TResult>(this IRfcContext context, string functionName, Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TRInput>> Input, Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TResult>> Output)
        {
            return context.CreateFunction(functionName).Use(
                ef => ef.Bind(func => 
                    
                    from input in Input(Prelude.Right(func)).ToAsync()
                    from _ in context.InvokeFunction(func)
                    from output in Output(Prelude.Right(func)).ToAsync()
                    select output)
                );

        }

        /// <summary>
        /// CallFunction with RfcErrorInfo lifted output.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="context"></param>
        /// <param name="functionName"></param>
        /// <param name="Output">Output function lifted in either monad.</param>
        /// <returns></returns>
        public static EitherAsync<RfcErrorInfo, TResult> CallFunction<TResult>(this IRfcContext context, string functionName, Func<Either<RfcErrorInfo,IFunction>, Either<RfcErrorInfo, TResult>> Output)
        {
            return context.CreateFunction(functionName).Use(
                ef => ef.Bind(func =>
                    from _ in context.InvokeFunction(func)
                    from output in Output(Prelude.Right(func)).ToAsync()
                    select output)
            );
        }

        /// <summary>
        /// CallFunction without input or output
        /// </summary>
        /// <param name="context"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public static EitherAsync<RfcErrorInfo, Unit> CallFunctionOneWay(this IRfcContext context, string functionName)
        {
            return context.CreateFunction(functionName).Use(
                func => func.Bind(context.InvokeFunction));

        }

        /// <summary>
        /// CallFunction with RfcInfo lifted input and no output
        /// </summary>
        /// <param name="context"></param>
        /// <param name="functionName"></param>
        /// <param name="Input">Input function lifted in either monad.</param>
        /// <returns></returns>
        public static EitherAsync<RfcErrorInfo, Unit> CallFunctionOneWay<TRInput>(this IRfcContext context, string functionName, Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TRInput>> Input)
        {
            return context.CreateFunction(functionName).Use(
                ef => ef.Bind(func =>

                    from input in Input(Prelude.Right(func)).ToAsync()
                    from _ in context.InvokeFunction(func)
                    select Unit.Default)
            );
        }

    }
}
