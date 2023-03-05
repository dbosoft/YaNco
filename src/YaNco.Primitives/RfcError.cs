using JetBrains.Annotations;
using LanguageExt;
using LanguageExt.Common;

namespace Dbosoft.YaNco;

public record RfcError(RfcErrorInfo RfcErrorInfo, Option<Error> Inner = new()) : Expected(RfcErrorInfo.Message, (int)RfcErrorInfo.Code, Inner)
{
    public static RfcError Ok => RfcErrorInfo.Ok().ToRfcError();

    public static RfcError EmptyResult => RfcErrorInfo.EmptyResult().ToRfcError();

    public static RfcError Error(string message, RfcRc rc = RfcRc.RFC_ILLEGAL_STATE) =>
        YaNco.RfcErrorInfo.Error(message, rc).ToRfcError();

    public static RfcError Error(RfcRc rc = RfcRc.RFC_ILLEGAL_STATE) => YaNco.RfcErrorInfo.Error(rc).ToRfcError();
}