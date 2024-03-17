using LanguageExt;

namespace Dbosoft.YaNco;

// ReSharper disable once InconsistentNaming
public interface HasSAPRfcData<RT> where RT : struct, HasSAPRfcData<RT>
{
    Eff<RT, SAPRfcDataIO> RfcDataEff { get; }

}
