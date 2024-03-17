using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

public interface HasSAPRfcConnection<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcConnectionIO> RfcConnectionEff { get; }
}