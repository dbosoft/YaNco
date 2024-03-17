using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;
using ExportMATMAS.MaterialMaster;

namespace ExportMATMAS;

public class SAPServerSettings : SAPRfcRuntimeSettings
{
    public TransactionManager<MaterialMasterRecord> TaManager { get; }

    public SAPServerSettings(
        IFieldMapper fieldMapper, RfcRuntimeOptions options,
        TransactionManager<MaterialMasterRecord> taManager)
        : base(null, fieldMapper, options)
    {
        TaManager = taManager;
    }

    public SAPServerSettings(ILogger? logger, 
        IFieldMapper fieldMapper, RfcRuntimeOptions options, 
        TransactionManager<MaterialMasterRecord> taManager) 
        : base(logger, fieldMapper, options)
    {
        TaManager = taManager;
    }
}