using System;
using Dbosoft.YaNco.Live;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

[PublicAPI]
public static class RfcContextRuntimeAccess
{
    /// <summary>
    /// This method is a helper method to execute an IO effect from a <see cref="IRfcContext"/>.
    /// The IO effect is executed with the <see cref="SAPRfcRuntime"/> of the connection behind the <see cref="IRfcContext"/>.
    /// </summary>
    /// <remarks>
    /// If you must use this method, please consider migrating to <see cref="IRfcContext{RT}"/>.
    /// where you can access the runtime directly via the <see cref="Prelude.runtime{RT}"/> method without any side effects.
    /// </remarks>
    /// <typeparam name="TR">The result type</typeparam>
    /// <param name="context">context to be used</param>
    /// <param name="ioFunc">function to construct the IO effect.</param>
    /// <returns>A <see cref="EitherAsync{RfcError,TR}"/> with any error as left state and <typeparamref name="TR"/> as right state.</returns>
    public static EitherAsync<RfcError, TR> RunIO<TR>(this IRfcContext context, Func<IConnection, Aff<SAPRfcRuntime, TR>> ioFunc)
    {
        
        // the runtime if RfcContext is always a SAPRfcRuntime
        return context.GetConnection().Bind(c =>
        {
            var runtime = (SAPRfcRuntime)c.ConnectionRuntime;
            return ioFunc(c).ToEither(runtime);
        });
    }

    /// <summary>
    /// This method is a helper method to execute an IO effect from a <see cref="IRfcContext"/>.
    /// The IO effect is executed with the <see cref="SAPRfcRuntime"/> of the connection behind the <see cref="IRfcContext"/>.
    /// </summary>
    /// <remarks>
    /// If you must use this method, please consider migrating to <see cref="IRfcContext{RT}"/>.
    /// where you can access the runtime directly via the <see cref="Prelude.runtime{RT}"/> method without any side effects.
    /// </remarks>
    /// <typeparam name="TR">The result type</typeparam>
    /// <param name="context">context to be used</param>
    /// <param name="ioFunc">function to construct the IO effect.</param>
    /// <returns>A <see cref="EitherAsync{RfcError,TR}"/> with any error as left state and <typeparamref name="TR"/> as right state.</returns>
    public static EitherAsync<RfcError, TR> RunIO<TR>(this IRfcContext context, Func<IConnection, Eff<SAPRfcRuntime, TR>> ioFunc)
    {

        // the runtime if RfcContext is always a SAPRfcRuntime
        return context.GetConnection().Bind(c =>
        {
            var runtime = (SAPRfcRuntime)c.ConnectionRuntime;

            return ioFunc(c).ToEither(runtime).ToAsync();
        });
    }
}