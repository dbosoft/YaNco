using System;
using System.Threading.Tasks;
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


        public static EitherAsync<RfcErrorInfo, IRfcServer> Start(this EitherAsync<RfcErrorInfo, IRfcServer> eitherServer)
        {
            return eitherServer.Bind(s => s.Start().Map(_ => s));
        }

        public static async Task<IRfcServer> StartOrException(this EitherAsync<RfcErrorInfo, IRfcServer> eitherServer)
        {
            var res = await eitherServer.Bind(s => s.Start()
                    .Map(_ => s))
                .MapLeft(l => l.Throw())
                .ToEither();

            return res.RightAsEnumerable().FirstOrDefault();
        }

        public static EitherAsync<RfcErrorInfo, IRfcServer> Stop(this EitherAsync<RfcErrorInfo, IRfcServer> eitherServer, int timeout = 0)
        {
            return eitherServer.Bind(s => s.Stop(timeout).Map(_ => s));
        }
    }
}