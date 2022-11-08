using System;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco
{
    public static class FunctionalServerExtensions
    {

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
            return self.Bind(p => p.Reply((o, f) => f));
        }



    }
}