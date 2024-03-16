namespace Dbosoft.YaNco;

public static class Deprecations
{
    public const string RfcRuntime =
        """
        RfcRuntime is deprecated and has been replaced by IO effects provided via SAPRfcRuntime.
        To access the IO effects you can use the Prelude.runtime<RT>() method, where RT is the type of the runtime you are using.
        Common used methods are also accessible via the SAPRfc<RT> class.
        """;
}