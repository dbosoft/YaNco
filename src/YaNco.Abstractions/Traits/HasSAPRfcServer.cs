using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface HasSAPRfcServer<RT>
    where RT : struct, HasSAPRfc<RT>
{
    Eff<RT, SAPRfcServerIO> RfcServerEff { get; }
}