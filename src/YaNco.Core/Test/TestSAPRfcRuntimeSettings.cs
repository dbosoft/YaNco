using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco.Test;

public class TestSAPRfcRuntimeSettings : SAPRfcRuntimeSettings
{
    public TestSAPRfcRuntimeSettings(ILogger logger, IFieldMapper fieldMapper, RfcRuntimeOptions tableOptions) : base(logger, fieldMapper, tableOptions)
    {
    }

    public static TestSAPRfcRuntimeSettings Empty() => new(null, null, new RfcRuntimeOptions());

}