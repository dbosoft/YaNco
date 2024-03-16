using System;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

[PublicAPI]
public readonly struct CalledFunction<RT> where RT : struct,
    HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasSAPRfcLogger<RT>, HasSAPRfcData<RT>
{
    public readonly IFunction Function;
    private readonly Func<IRfcContext<RT>> _rfcContextFunc;

    internal CalledFunction(IRfcHandle rfcHandle, IFunction function,
        Func<IRfcContext<RT>> rfcContextFunc)
    {
        RfcHandle = rfcHandle;
        Function = function;
        _rfcContextFunc = rfcContextFunc;
    }

    /// <summary>
    /// Input processing for a called function. Use this method to extract values from the rfc function.
    /// </summary>
    /// <typeparam name="TInput">Type of data extracted from function. Could be any type.</typeparam>
    /// <param name="inputFunc">Function to map from RFC function to the desired input type</param>
    /// <returns><see cref="FunctionInput{TInput}"/> wrapped in a <see cref="Either{L,R}"/> </returns>
    public Either<RfcError, FunctionInput<RT,TInput>> Input<TInput>(Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> inputFunc)
    {
        var function = Function;
        return inputFunc(Prelude.Right(function)).Map(input => new FunctionInput<RT,TInput>(input, function));
    }

    public Eff<RT,TR> UseRfcContext<TR>(Func<IRfcContext<RT>, Eff<RT,TR>> mapFunc)
    {
        var func = _rfcContextFunc;
        return Prelude.use(Prelude.Eff<RT, IRfcContext<RT>>( _ => func()),mapFunc);
    }

    public Aff<RT,TR> UseRfcContext<TR>(Func<IRfcContext<RT>, Aff<RT,TR>> mapFunc)
    {
        var func = _rfcContextFunc;
        return Prelude.use(Prelude.Eff<RT, IRfcContext<RT>>(_ => func()), mapFunc);
    }


    public readonly IRfcHandle RfcHandle;


}