using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt;
using LanguageExt.Attributes;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

[Typeclass("*")]
public interface HasRfcContext<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, IRfcContext> RfcContextEff { get; }

}

public static class RfcContext<RT>
    where RT : struct, HasRfcContext<RT>
{
    [Pure, MethodImpl(AffOpt.mops)]
    public static Aff<RT,TResult> callFunction<TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        default(RT).RfcContextEff.Bind(
            context => context.CreateFunction(functionName).Use(
                ef => ef.Bind(func =>
                    from _ in context.InvokeFunction(func, default(RT).CancellationToken)
                    from output in Output(Prelude.Right(func)).ToAsync()
                    select output)).ToAff(l=>l));

    public static Aff<RT,TResult> callFunction<TInput, TResult>(
        string functionName,
        Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> Input,
        Func<Either<RfcError, IFunction>, Either<RfcError, TResult>> Output) =>
        default(RT).RfcContextEff.Bind(
            context => context.CreateFunction(functionName).Use(
                ef => ef.Bind(func =>
                    from input in Input(Prelude.Right(func)).ToAsync()
                    from _ in context.InvokeFunction(func, default(RT).CancellationToken)
                    from output in Output(Prelude.Right(func)).ToAsync()
                    select output)).ToAff(l => l));

}
