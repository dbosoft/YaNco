using LanguageExt;

namespace Dbosoft.YaNco;

public interface HasSAPRfcData<RT> where RT : struct, HasSAPRfcData<RT>
{
    Eff<RT, SAPRfcDataIO> RfcDataEff { get; }

}
