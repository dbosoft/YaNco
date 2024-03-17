using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

public interface HasSAPRfcServer<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcServerIO> RfcServerEff { get; }
}