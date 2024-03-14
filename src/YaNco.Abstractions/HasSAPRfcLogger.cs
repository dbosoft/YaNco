using LanguageExt;

namespace Dbosoft.YaNco;

public interface HasSAPRfcLogger<RT> where RT : struct, HasSAPRfcLogger<RT>
{
    Eff<RT, Option<ILogger>> RfcLoggerEff { get; }

}