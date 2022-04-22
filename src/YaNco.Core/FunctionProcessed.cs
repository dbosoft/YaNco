using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public readonly struct FunctionProcessed<TOutput>
    {
        private readonly TOutput Output;
        private readonly IFunction Function;

        internal FunctionProcessed(TOutput output, IFunction function)
        {
            Output = output;
            Function = function;
        }

        public Either<RfcErrorInfo,Unit> Reply(Func<TOutput, Either<RfcErrorInfo, IFunction>, Either<RfcErrorInfo, IFunction>> replyFunc)
        {
            return replyFunc(Output, Prelude.Right(Function)).Map(_=>Unit.Default);
        }
    }
}