using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco.Traits;

[PublicAPI]
// ReSharper disable once InconsistentNaming
public interface HasSAPRfcLibrary<RT> : IHasEnvRuntimeSettings
    where RT : struct
{
    Eff<RT, SAPRfcLibraryIO> RfcLibraryEff { get; }

}