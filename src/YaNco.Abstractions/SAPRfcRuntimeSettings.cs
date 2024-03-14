using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco;

public class SAPRfcRuntimeSettings
{
    public readonly IFieldMapper FieldMapper;
    public readonly ILogger Logger;
    public readonly RfcTableOptions TableOptions;

    public SAPRfcRuntimeSettings(ILogger logger, IFieldMapper fieldMapper, RfcTableOptions tableOptions)
    {
        FieldMapper = fieldMapper;
        Logger = logger;
        TableOptions = tableOptions;
    }

}