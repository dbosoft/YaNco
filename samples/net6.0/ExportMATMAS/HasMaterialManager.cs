using ExportMATMAS.MaterialMaster;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace ExportMATMAS;

public interface HasMaterialManager<RT> : HasCancel<RT>
    where RT : struct, HasCancel<RT>
{
    Eff<RT, TransactionManager<MaterialMasterRecord>> MaterialManagerEff { get; }
}