using System;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public readonly struct CalledFunction<RT, TSettings> where RT : struct, HasCancelFactory<RT>,
        HasSAPRfcServer<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasEnvSettings<TSettings>
        where TSettings : SAPRfcRuntimeSettings
    {
        public readonly IFunction Function;
        private readonly Func<IRfcContext> _rfcContextFunc;
        private readonly Func<CancellationToken, RT> _runtimeFunc;

        internal CalledFunction(IRfcHandle rfcHandle, IFunction function,
        Func<IRfcContext> rfcContextFunc,
        Func<CancellationToken, RT> runtimeFunc)
        {
            RfcHandle = rfcHandle;
            Function = function;
            _rfcContextFunc = rfcContextFunc;
            _runtimeFunc = runtimeFunc;
        }

        /// <summary>
        /// Input processing for a called function. Use this method to extract values from the rfc function.
        /// </summary>
        /// <typeparam name="TInput">Type of data extracted from function. Could be any type.</typeparam>
        /// <param name="inputFunc">Function to map from RFC function to the desired input type</param>
        /// <returns><see cref="FunctionInput{TInput}"/> wrapped in a <see cref="Either{L,R}"/> </returns>
        public Either<RfcError, FunctionInput<TInput>> Input<TInput>(Func<Either<RfcError, IFunction>, Either<RfcError, TInput>> inputFunc)
        {
            var function = Function;
            return inputFunc(Prelude.Right(function)).Map(input => new FunctionInput<TInput>(input, function));
        }

        public T UseRfcContext<T>(Func<IRfcContext, T> mapFunc)
        {
            using var rfcContext = _rfcContextFunc();
            return mapFunc(rfcContext);
        }

        public async Task<T> UseRfcContextAsync<T>(Func<IRfcContext, Task<T>> mapFunc)
        {
            using var rfcContext = _rfcContextFunc();
            return await mapFunc(rfcContext);
        }


        public readonly IRfcHandle RfcHandle;

        public Either<RfcError, RfcServerAttributes> GetServerAttributes(CancellationToken cancellationToken = default)
        {
            var handle = RfcHandle;
            return default(RT).RfcServerEff.Bind(io => io.GetServerCallContext(handle).ToEff(l => l))
                .ToEither(_runtimeFunc(cancellationToken));
        }

        [Obsolete("Obsolete")]
        public IRfcRuntime RfcRuntime => new RfcRuntime<RT, TSettings>(_runtimeFunc(CancellationToken.None));
    }
}