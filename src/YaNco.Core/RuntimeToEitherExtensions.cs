using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

internal static class RuntimeToEitherExtensions
{
    public static Either<RfcError, T> ToEither<T,RT>(this Eff<RT, T> eff, RT runtime)
       where RT : struct
    {
        return eff.Run(runtime).ToEither().ToRfcError();

    }
    public static EitherAsync<RfcError, T> ToEither<RT,T>(this Aff<RT, T> aff, RT runtime)
        where RT : struct, HasCancel<RT>
    {
        return Run().Map(fin => fin.ToEither().ToRfcError()).ToAsync();

        async Task<Fin<T>> Run()
        {
            return await aff.Run(runtime).ConfigureAwait(false);

        }
    }

    public static Either<RfcError, T> ToRfcError<T>(this Either<Error, T> either) =>
        either.MapLeft(RfcError.New);
}