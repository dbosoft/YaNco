using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public readonly struct CalledFunction
    {
        public readonly IFunction Function;

        internal CalledFunction(IFunction function)
        {
            Function = function;
        }

        public Either<RfcErrorInfo,FunctionInput<TInput>> Input<TInput>(Func<Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, TInput>> inputFunc)
        {
            var function = Function;
            return inputFunc(Prelude.Right(function)).Map(input => new FunctionInput<TInput>(input, function));
        }


    }
}