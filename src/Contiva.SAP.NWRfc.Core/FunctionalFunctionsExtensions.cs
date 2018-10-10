using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public static class FunctionalFunctionsExtensions
    {

        public static Task<Either<RfcErrorInfo, R1>> Commit<R1>(this Task<Either<RfcErrorInfo, R1>> self,
            IRfcContext context)
        {
            return self.BindAsync(res => context.Commit().MapAsync(u => res));
        }

        public static Task<Either<RfcErrorInfo, R1>> CommitAndWait<R1>(this Task<Either<RfcErrorInfo, R1>> self,
            IRfcContext context)
        {
            return self.BindAsync(res => context.CommitAndWait().MapAsync(u => res));
        }

        public static Task<Either<RfcErrorInfo, IFunction>> HandleReturn(this Task<Either<RfcErrorInfo, IFunction>> self)
        {
            return self.BindAsync(f =>
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
                select f);

        }

        private static Either<RfcErrorInfo, TResult> ErrorOrResult<TResult>(TResult result, string type, string id, string number, string message, string v1, string v2, string v3, string v4)
        {
            if (type.Contains('E') || type.Contains('A'))
                return new RfcErrorInfo(RfcRc.RFC_ABAP_MESSAGE, RfcErrorGroup.ABAP_APPLICATION_FAILURE, "", message, id, type, number, v1, v2, v3, v4);

            return result;
        }

        // ReSharper disable InconsistentNaming
        public static Task<Either<RfcErrorInfo, TResult>> CallFunction<TRInput,TResult>(this IRfcContext context, string functionName, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TRInput>>> Input, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TResult>>> Output)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Apply(Input).BindAsync(i=>func)
                    .BindAsync(context.InvokeFunction).Map(i => func)
                    .Apply(Output));

        }

        public static Task<Either<RfcErrorInfo, TResult>> CallFunction<TResult>(this IRfcContext context, string functionName, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TResult>>> Output)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .BindAsync(context.InvokeFunction).Map(i => func)
                    .Apply(Output));
        }

        public static Task<Either<RfcErrorInfo, Unit>> CallFunctionAsUnit<TRInput>(this IRfcContext context, string functionName, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TRInput>>> Input)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Apply(Input).BindAsync(i => func)
                    .BindAsync(context.InvokeFunction));
        }

        public static Task<Either<RfcErrorInfo, Unit>> CallFunctionAsUnit<TRInput,TResult>(this IRfcContext context, string functionName, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TRInput>>> Input, Func<Task<Either<RfcErrorInfo, IFunction>>, Task<Either<RfcErrorInfo, TResult>>> Output)
        {
            return context.CreateFunction(functionName).Use(
                func => func
                    .Apply(Input).BindAsync(i => func)
                    .BindAsync(context.InvokeFunction).Map(i => func)
                    .Apply(Output)).MapAsync(f => Unit.Default);

        }

        public static Task<Either<RfcErrorInfo, Unit>> CallFunction(this IRfcContext context, string functionName)
        {
            return context.CreateFunction(functionName).Use(
                func => func.BindAsync(context.InvokeFunction));

        }
    }
}
