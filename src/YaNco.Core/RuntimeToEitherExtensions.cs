using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;

namespace Dbosoft.YaNco;

internal static class RuntimeToEitherExtensions
{
    public static EitherAsync<RfcError, T> ToEither<T>(this Aff<SAPRfcRuntime, T> aff, SAPRfcRuntime runtime)
    {
        return Run().Map(fin => fin.ToEither().ToRfcError()).ToAsync();

        async Task<Fin<T>> Run()
        {
            return await aff.Run(runtime);

        }
    }

    public static Either<RfcError, T> ToRfcError<T>(this Either<Error, T> either) =>
        either.MapLeft(RfcError.New);
}