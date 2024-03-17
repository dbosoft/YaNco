using Dbosoft.YaNco.Traits;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco;

public class SAPRfcRuntimeSettings
{
    public readonly IFieldMapper FieldMapper;
    public readonly ILogger Logger;
    public readonly RfcRuntimeOptions Options = new();

    public SAPRfcRuntimeSettings(IFieldMapper fieldMapper)
    {
        FieldMapper = fieldMapper;
    }

    public SAPRfcRuntimeSettings(ILogger logger, IFieldMapper fieldMapper, RfcRuntimeOptions options)
    {
        FieldMapper = fieldMapper;
        Logger = logger;
        Options = options;
    }

    public SAPRfcDataIO RfcDataIO { get; set; }
    public SAPRfcFunctionIO RfcFunctionIO { get; set; }
    public SAPRfcConnectionIO RfcConnectionIO { get; set; }
    public SAPRfcServerIO RfcServerIO { get; set; }
}