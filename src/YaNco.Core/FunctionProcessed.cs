using System;
using LanguageExt;

namespace Dbosoft.YaNco;

public readonly struct FunctionProcessed<TOutput>
{
    private readonly TOutput _output;
    private readonly IFunction _function;

    internal FunctionProcessed(TOutput output, IFunction function)
    {
        _output = output;
        _function = function;
    }

    public Either<RfcError, Unit> Reply(Func<TOutput, Either<RfcError, IFunction>, Either<RfcError, IFunction>> replyFunc)
    {
        return replyFunc(_output, Prelude.Right(_function)).Map(_ => Unit.Default);
    }
}