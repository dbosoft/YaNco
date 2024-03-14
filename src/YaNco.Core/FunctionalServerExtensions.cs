using System;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco
{
    public static class FunctionalServerExtensions
    {
        /// <summary>
        /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
        /// </summary>
        /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
        /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
        /// <param name="input">input from previous chain step.</param>
        /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
        /// <returns><see cref="FunctionProcessed{TOutput}"/></returns>
        public static Either<RfcError, FunctionProcessed<TOutput>> Process<TInput, TOutput>(
            this Either<RfcError, FunctionInput<TInput>> input,
            Func<TInput, TOutput> processFunc)
        {
            return input.Map(i => i.Process(processFunc));
        }

        /// <summary>
        /// Async data processing for a called function. Use this method to do any work you would like to do when the function is called.
        /// </summary>
        /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
        /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
        /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
        /// <param name="input">input from previous chain step.</param>
        /// <returns><see cref="FunctionProcessed{TOutput}"/></returns>
        public static EitherAsync<RfcError, FunctionProcessed<TOutput>> ProcessAsync<TInput, TOutput>(
            this Either<RfcError, FunctionInput<TInput>> input,
            Func<TInput, Task<TOutput>> processFunc)
        {
            return input.ToAsync()
                .MapAsync(i => i.ProcessAsync(processFunc));
        }

        /// <summary>
        /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
        /// </summary>
        /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
        /// <param name="input">input from previous chain step.</param>
        /// <param name="processAction">Action to process <typeparamref name="TInput"></typeparamref>.</param>
        /// <returns><see cref="FunctionProcessed{Unit}"/>"/></returns>
        public static Either<RfcError, FunctionProcessed<Unit>> Process<TInput>(
            this Either<RfcError, FunctionInput<TInput>> input,
            Action<TInput> processAction)
        {
            return input.Map(i =>
            {
                var (function, input1) = i;
                processAction(input1);
                return new FunctionProcessed<Unit>(Unit.Default, function);
            });
        }

        /// <summary>
        /// Use this method to send a response to sap backend after processing the function call.
        /// </summary>
        /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
        /// <param name="self">previous chain step</param>
        /// <param name="replyFunc">Function to map from <typeparam name="TOutput"/> to rfc function.</param>
        /// <returns></returns>
        public static EitherAsync<RfcError, Unit> Reply<TOutput>(this EitherAsync<RfcError, FunctionProcessed<TOutput>> self, 
            Func<TOutput, Either<RfcError, IFunction>, Either<RfcError, IFunction>> replyFunc)
        {
            return self.Bind(p => p.Reply(replyFunc).ToAsync());
        }

        /// <summary>
        /// Use this method to send a response to sap backend after processing the function call.
        /// </summary>
        /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
        /// <param name="self">previous chain step</param>
        /// <param name="replyFunc">Function to map from <typeparam name="TOutput"/> to rfc function.</param>
        /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
        public static EitherAsync<RfcError, Unit> Reply<TOutput>(this Either<RfcError, FunctionProcessed<TOutput>> self, 
            Func<TOutput, Either<RfcError, IFunction>, Either<RfcError, IFunction>> replyFunc)
        {
            return self.Bind(p => p.Reply(replyFunc)).ToAsync();
        }

        /// <summary>
        /// Use this method to end the function call chain without sending a response back to SAP backend.
        /// </summary>
        /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
        /// <param name="self">previous chain step</param>
        /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
        public static EitherAsync<RfcError, Unit> NoReply<TOutput>(this Either<RfcError, FunctionProcessed<TOutput>> self)
        {
            return self.Bind(p => p.Reply((o, f) => f)).ToAsync();
        }

        /// <summary>
        /// Use this method to end the function call chain without sending a response back to SAP backend.
        /// </summary>
        /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
        /// <param name="self">previous chain step</param>
        /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
        public static EitherAsync<RfcError, Unit> NoReply<TOutput>(this EitherAsync<RfcError, FunctionProcessed<TOutput>> self)
        {
            return self.Bind(p => p.Reply((o, f) => f).ToAsync());
        }

        /// <summary>
        /// Starts a RFC Server
        /// </summary>
        /// <param name="eitherServer">RFC Server, wrapping in a <see cref="EitherAsync{RfcError,IRfcServer}"/></param>
        /// <returns>started RFC Server, wrapping in a <see cref="EitherAsync{RfcError,IRfcServer}"/></returns>
        public static EitherAsync<RfcError, IRfcServer<RT>> Start<RT>(this EitherAsync<RfcError, IRfcServer<RT>> eitherServer) 
                where RT : struct, HasCancel<RT>
        {
            return eitherServer.Bind(s => s.Start().Map(_ => s));
        }

        /// <summary>
        /// Starts a RFC Server or throws a <see cref="RfcErrorException"/> if start failed.
        /// </summary>
        /// <param name="eitherServer">RFC Server, wrapping in a <see cref="EitherAsync{RfcError,IRfcServer}"/></param>
        /// <returns>Started RFC Server if Either is not left. Otherwise a exception will be thrown.</returns>
        /// <remarks>Use this method to integrate the <see cref="IRfcServer"/> with services that expect exceptions on failure.</remarks>
        /// <exception cref="RfcErrorException"></exception>
        public static async Task<IRfcServer<RT>> StartOrException<RT>(this EitherAsync<RfcError, IRfcServer<RT>> eitherServer) 
            where RT : struct, HasCancel<RT>
        {
            var res = await eitherServer.Bind(s => s.Start()
                    .Map(_ => s))
                .MapLeft(l => l.Throw())
                .ToEither();

            return res.RightAsEnumerable().FirstOrDefault();
        }

        /// <summary>
        /// Stops a RFC Server
        /// </summary>
        /// <param name="eitherServer">RFC Server, wrapping in a <see cref="EitherAsync{RfcError,IRfcServer}"/></param>
        /// <param name="timeout">Timeout for stopping the rfc server</param>
        /// <returns>Stopped RFC Server, wrapping in a <see cref="EitherAsync{RfcError,IRfcServer}"/></returns>
        public static EitherAsync<RfcError, IRfcServer<RT>> Stop<RT>(this EitherAsync<RfcError, IRfcServer<RT>> eitherServer, int timeout = 0) where RT : struct, HasCancel<RT>
        {
            return eitherServer.Bind(s => s.Stop(timeout).Map(_ => s));
        }
    }
}