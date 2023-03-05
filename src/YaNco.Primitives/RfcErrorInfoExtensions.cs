namespace Dbosoft.YaNco;

public static class RfcErrorInfoExtensions
{
    public static RfcError ToRfcError(this RfcErrorInfo rfcErrorInfo)
    {
        return new RfcError(rfcErrorInfo);
    }
}