using LanguageExt;
using LanguageExt.Effects.Traits;

// ReSharper disable InconsistentNaming
#pragma warning disable IDE1006

namespace Dbosoft.YaNco;


public interface HasSAPRfcFunctions<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, SAPRfcFunctionIO> RfcFunctionsEff { get; }
}