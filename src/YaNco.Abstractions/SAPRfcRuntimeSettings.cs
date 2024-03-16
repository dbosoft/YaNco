using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco;

public class SAPRfcRuntimeSettings
{
    public readonly IFieldMapper FieldMapper;
    public readonly ILogger Logger;
    public readonly RfcRuntimeOptions TableOptions;

    public SAPRfcRuntimeSettings(ILogger logger, IFieldMapper fieldMapper, RfcRuntimeOptions tableOptions)
    {
        FieldMapper = fieldMapper;
        Logger = logger;
        TableOptions = tableOptions;
    }

    public SAPRfcDataIO RfcDataIO { get; set; }
    public SAPRfcFunctionIO RfcFunctionIO { get; set; }
    public SAPRfcConnectionIO RfcConnectionIO { get; set; }
    public SAPRfcServerIO RfcServerIO { get; set; }
}