namespace Dbosoft.YaNco;

public static class Deprecations
{
    public const string RfcRuntime =
        """
        RfcRuntime is deprecated and has been replaced by IO effects provided via SAPRfcRuntime.
        If you are using a RfcContext you should consider migrating to RfcContext<RT>. 
        Then can use the Prelude.runtime<RT>() method, where RT is the type of the runtime you are using.
        Common used methods are also accessible via the SAPRfc<RT> class.
        
        If you don't want to migrate to RfcContext<RT> you can also use the extension method 
        GetSAPRfcRuntime() that accesses the runtime of the connection within the context.       
        """;
}