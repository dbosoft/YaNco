using LanguageExt;

namespace Dbosoft.YaNco.Live;

public interface HasSAPRfcData<RT> where RT : struct, HasSAPRfcData<RT>
{
    Eff<RT, SAPRfcDataIO> RfcDataEff { get; }

}