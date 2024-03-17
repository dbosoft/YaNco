using Dbosoft.YaNco.TypeMapping;
using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
[PublicAPI]
public interface HasSAPRfc<RT> : IHasEnvRuntimeSettings
    where RT : struct
{
    Eff<RT, SAPRfcConnectionIO> RfcConnectionEff { get; }
    Eff<RT, SAPRfcDataIO> RfcDataEff { get; }
    Eff<RT, SAPRfcFunctionIO> RfcFunctionsEff { get; }
    Eff<RT, Option<ILogger>> RfcLoggerEff { get; }
    Eff<RT, IFieldMapper> FieldMapperEff { get; }

}