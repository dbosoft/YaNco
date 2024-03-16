namespace Dbosoft.YaNco;

public static class Deprecations
{
    public const string RfcRuntime =
        """
        RfcRuntime is deprecated and has been replaced by IO effects in a functional runtime.
        If you are using an IRfcContext, you should consider migrating to IRfcContext<RT>. 
        Then you can use the Prelude.runtime<RT>() method, where RT is the type of runtime you are using.
        Commonly used methods are also accessible through the SAPRfc<RT> class.

        If you don't want to migrate to RfcContext<RT>, you can also use the extension method 
        RunIO() to execute an IO effect within an RfcContext.
        """;
}