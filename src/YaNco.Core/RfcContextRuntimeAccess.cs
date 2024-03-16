using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco;

public static class RfcContextRuntimeAccess
{
    /// <summary>
    /// This method is a helper method to get the <see cref="SAPRfcRuntime"/> from a <see cref="IRfcContext"/>.
    /// If you have to use this method please consider migrating to <see cref="IRfcContext{RT}"/>
    /// where you can access the runtime directly via the <see cref="Prelude.runtime{RT}"/> method.
    /// </summary>
    /// <param name="context">context to be used</param>
    /// <returns><see cref="SAPRfcRuntime"/> that can be used to t run IO effects within the <see cref="IRfcContext{RT}"/></returns>
    public static EitherAsync<RfcError, SAPRfcRuntime> GetSAPRfcRuntime(this IRfcContext context)
    {
        // the runtime if RfcContext is always a SAPRfcRuntime
        return context.GetConnection().Map(c => (SAPRfcRuntime) c.ConnectionRuntime);
    }
}