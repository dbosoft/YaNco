using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco.Traits;

public interface HasSAPRfcServer<RT>
    where RT : struct, HasSAPRfc<RT>
{
    Eff<RT, SAPRfcServerIO> RfcServerEff { get; }
}