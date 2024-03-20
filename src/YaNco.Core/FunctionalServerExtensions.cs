using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco;

[PublicAPI]
public static class FunctionalServerExtensions
{
    /// <summary>
    /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
    /// <typeparam name="RT">the runtime for the call</typeparam>
    /// <param name="input">input from previous chain step.</param>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/></returns>
    public static Eff<RT, FunctionProcessed<TOutput>> Process<RT,TInput, TOutput>(
        this Either<RfcError, FunctionInput<RT,TInput>> input,
        Func<TInput, Eff<RT,TOutput>> processFunc) where RT : struct, HasCancel<RT>
    {
        return input.ToEff(l=>l).Bind(i => i.Process(processFunc));
    }

    /// <summary>
    /// Async data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
    /// <typeparam name="RT">the runtime</typeparam>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <param name="input">input from previous chain step.</param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/></returns>
    public static Aff<RT, FunctionProcessed<TOutput>> Process<RT,TInput, TOutput>(
        this Either<RfcError, FunctionInput<RT,TInput>> input,
        Func<TInput, Aff<RT,TOutput>> processFunc) where RT : struct, HasCancel<RT>
    {
        return input.ToAff(l=>l)
            .Bind(i => i.Process(processFunc));
    }

    /// <summary>
    /// Async data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TOutput">Type of data returned from processing. Could be any type.</typeparam>
    /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
    /// <typeparam name="RT">the runtime</typeparam>
    /// <param name="processFunc">Function to map from <typeparamref name="TInput"></typeparamref> to <typeparam name="TOutput"></typeparam>"/></param>
    /// <param name="input">input from previous chain step.</param>
    /// <returns><see cref="FunctionProcessed{TOutput}"/>in a <see cref="Aff{RT,A}"/></returns>
    public static Aff<RT, FunctionProcessed<TOutput>> ProcessAsync<RT, TInput, TOutput>(
        this Either<RfcError, FunctionInput<RT, TInput>> input,
        Func<TInput, ValueTask<TOutput>> processFunc) where RT : struct, HasCancel<RT>
    {
        return input.ToAff(l => l)
            .Bind(i => i.ProcessAsync(processFunc));
    }

    /// <summary>
    /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
    /// <typeparam name="RT">the runtime</typeparam>
    /// <param name="input">input from previous chain step.</param>
    /// <param name="processAction">Action to process <typeparamref name="TInput"></typeparamref>.</param>
    /// <returns><see cref="FunctionProcessed{Unit}"/>"/> in a <see cref="Eff{RT,A}"/></returns>
    public static Eff<RT, FunctionProcessed<Unit>> Process<RT,TInput>(
        this Either<RfcError, FunctionInput<RT,TInput>> input,
        Action<TInput> processAction) where RT : struct, HasCancel<RT>
    {
        return input.Map(i =>
        {
            var (function, input1) = i;
            processAction(input1);
            return new FunctionProcessed<Unit>(Unit.Default, function);
        }).ToEff(l=>l);
    }

    /// <summary>
    /// Data processing for a called function. Use this method to do any work you would like to do when the function is called.
    /// </summary>
    /// <typeparam name="TInput">Type of data returned from rfc input. Could be any type.</typeparam>
    /// <typeparam name="RT">the runtime</typeparam>
    /// <param name="input">input from previous chain step.</param>
    /// <param name="processFunc">Function to process <typeparamref name="TInput"></typeparamref> that returns a <see cref="ValueTask"/>.</param>
    /// <returns><see cref="FunctionProcessed{Unit}"/>"/> in a <see cref="Eff{RT,A}"/></returns>

    public static Aff<RT, FunctionProcessed<Unit>> ProcessAsync<RT, TInput>(
        this Either<RfcError, FunctionInput<RT, TInput>> input,
        Func<TInput,ValueTask> processFunc) where RT : struct, HasCancel<RT>
    {
        return input.ToEff(l => l).Bind(i => Prelude.Aff(async () =>
        {
            var (function, input1) = i;
            await processFunc(input1).ConfigureAwait(false);
            return new FunctionProcessed<Unit>(Unit.Default, function);
        }));
    }

    /// <summary>
    /// Use this method to send a response to sap backend after processing the function call.
    /// </summary>
    /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
    /// <typeparam name="RT">runtime used</typeparam>
    /// <param name="self">previous chain step</param>
    /// <param name="replyFunc">Function to map from <typeparam name="TOutput"/> to rfc function.</param>
    /// <returns></returns>
    public static Eff<RT, Unit> Reply<RT,TOutput>(this Eff<RT, FunctionProcessed<TOutput>> self, 
        Func<TOutput, Either<RfcError, IFunction>, Either<RfcError, IFunction>> replyFunc) where RT : struct
    {
        return self.Bind(p => p.Reply(replyFunc).ToEff(l=>l));
    }

    /// <summary>
    /// Use this method to send a response to sap backend after processing the function call.
    /// </summary>
    /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
    /// <typeparam name="RT">runtime used</typeparam>
    /// <param name="self">previous chain step</param>
    /// <param name="replyFunc">Function to map from <typeparam name="TOutput"/> to rfc function.</param>
    /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
    public static Aff<RT, Unit> Reply<RT,TOutput>(this Aff<RT, FunctionProcessed<TOutput>> self, 
        Func<TOutput, Either<RfcError, IFunction>, Either<RfcError, IFunction>> replyFunc) where RT : struct, HasCancel<RT>
    {
        return self.Bind(p => p.Reply(replyFunc).ToEff(l=>l));
    }

    /// <summary>
    /// Use this method to end the function call chain without sending a response back to SAP backend.
    /// </summary>
    /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
    /// <typeparam name="RT">runtime used</typeparam>
    /// <param name="self">previous chain step</param>
    /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
    public static Eff<RT, Unit> NoReply<RT,TOutput>(this Eff<RT, FunctionProcessed<TOutput>> self) where RT : struct
    {
        return self.Bind(p => p.Reply((_, f) => f).ToEff(l=>l));
    }

    /// <summary>
    /// Use this method to end the function call chain without sending a response back to SAP backend.
    /// </summary>
    /// <typeparam name="TOutput">type of Output of processing chain step.</typeparam>
    /// <typeparam name="RT">runtime used</typeparam>
    /// <param name="self">previous chain step</param>
    /// <returns><see cref="Unit"/> wrapped in a <see cref="EitherAsync{RfcError,Unit}"/></returns>
    public static Aff<RT, Unit> NoReply<RT,TOutput>(this Aff<RT, FunctionProcessed<TOutput>> self) where RT : struct, HasCancel<RT>
    {
        return self.Bind(p => p.Reply((_, f) => f).ToAff(l=>l));
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
    /// <remarks>Use this method to integrate the <see cref="IRfcServer{RT}"/> with services that expect exceptions on failure.</remarks>
    /// <exception cref="RfcErrorException"></exception>
    public static async Task<IRfcServer<RT>> StartOrException<RT>(this EitherAsync<RfcError, IRfcServer<RT>> eitherServer) 
        where RT : struct, HasCancel<RT>
    {
        var res = await eitherServer.Bind(s => s.Start()
                .Map(_ => s))
            .MapLeft(l => l.Throw())
            .ToEither().ConfigureAwait(false);

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