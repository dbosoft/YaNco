using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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
                select f).ToAsync());

        }

        private static Either<RfcErrorInfo, TResult> ErrorOrResult<TResult>(TResult result, string type, string id, string number, string message, string v1, string v2, string v3, string v4)
        {
            if (type.Contains('E') || type.Contains('A'))
                return new RfcErrorInfo(RfcRc.RFC_ABAP_MESSAGE, RfcErrorGroup.ABAP_APPLICATION_FAILURE, "", message, id, type, number, v1, v2, v3, v4);

            return result;
        }

        // ReSharper disable InconsistentNaming

        public static EitherAsync<RfcErrorInfo, TResult> CallFunction<TRInput, TResult>(this IRfcContext context,
            string functionName, Func<EitherAsync<RfcErrorInfo, IFunction>, EitherAsync<RfcErrorInfo, TRInput>> Input,
            Func<IFunction, Either<RfcErrorInfo,TResult>> Output)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Apply(Input).Bind(i => func)
                    .Bind(context.InvokeFunction).Bind(i => func)
                    .Bind(f=> Output(f).ToAsync()));
        }

        public static Task<Either<RfcErrorInfo, TResult>> CallFunctionAsync<TRInput, TResult>(this IRfcContext context,
            string functionName, Func<EitherAsync<RfcErrorInfo, IFunction>, EitherAsync<RfcErrorInfo, TRInput>> Input,
            Func<IFunction, Either<RfcErrorInfo, TResult>> Output)
        {
            return CallFunction(context, functionName, Input, Output).ToEither();
        }

        public static EitherAsync<RfcErrorInfo, TResult> CallFunction<TResult>(this IRfcContext context, string functionName, Func<IFunction, Either<RfcErrorInfo, TResult>> Output)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Bind(context.InvokeFunction).Bind(i => func)
                    .Bind(f => Output(f).ToAsync()));
        }

        public static Task<Either<RfcErrorInfo, TResult>> CallFunctionAsync<TResult>(this IRfcContext context,
            string functionName, Func<IFunction, Either<RfcErrorInfo, TResult>> Output)
        {
            return CallFunction(context, functionName, Output).ToEither();
        }

        public static EitherAsync<RfcErrorInfo, Unit> CallFunctionAsUnit<TRInput>(this IRfcContext context, string functionName, Func<EitherAsync<RfcErrorInfo, IFunction>, EitherAsync<RfcErrorInfo, TRInput>> Input)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Apply(Input).Bind(i => func)
                    .Bind(context.InvokeFunction));
        }

        public static Task<Either<RfcErrorInfo, Unit>> CallFunctionAsUnitAsync<TRInput>(this IRfcContext context,
            string functionName, Func<EitherAsync<RfcErrorInfo, IFunction>, EitherAsync<RfcErrorInfo, TRInput>> Input)
        {
            return CallFunctionAsUnit(context, functionName, Input).ToEither();
        }

        public static EitherAsync<RfcErrorInfo, Unit> CallFunction(this IRfcContext context, string functionName)
        {
            return context.CreateFunction(functionName).Use(
                func => func.Bind(context.InvokeFunction));

        }

        public static Task<Either<RfcErrorInfo, Unit>> CallFunctionAsync(this IRfcContext context, string functionName)
        {
            return CallFunction(context, functionName).ToEither();
        }
    }
}
