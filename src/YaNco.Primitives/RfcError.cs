using System;
using LanguageExt;
using LanguageExt.Common;

namespace Dbosoft.YaNco;

public record RfcError(RfcErrorInfo RfcErrorInfo, Option<Error> Inner = new()) : Expected(RfcErrorInfo.Message, (int)RfcErrorInfo.Code, Inner)
{
    public RfcRc Rc => RfcErrorInfo.Code;

    public RfcErrorGroup Group => RfcErrorInfo.Group;

    public string Key => RfcErrorInfo.Key;

    public string AbapMsgClass => RfcErrorInfo.AbapMsgClass;

    public string AbapMsgType => RfcErrorInfo.AbapMsgType;

    public string AbapMsgNumber => RfcErrorInfo.AbapMsgNumber;

    public string AbapMsgV1 => RfcErrorInfo.AbapMsgV1;

    public string AbapMsgV2 => RfcErrorInfo.AbapMsgV2;

    public string AbapMsgV3 => RfcErrorInfo.AbapMsgV3;

    public string AbapMsgV4 => RfcErrorInfo.AbapMsgV4;

    public static RfcError Ok => RfcErrorInfo.Ok().ToRfcError();

    public static RfcError EmptyResult => RfcErrorInfo.EmptyResult().ToRfcError();

    public static RfcError Error(string message, RfcRc rc = RfcRc.RFC_ILLEGAL_STATE) =>
        YaNco.RfcErrorInfo.Error(message, rc).ToRfcError();

    public static RfcError Error(RfcRc rc = RfcRc.RFC_ILLEGAL_STATE) => RfcErrorInfo.Error(rc).ToRfcError();

    public static RfcError New(Error error)
    {
        if(error is RfcError rfcError)
            return rfcError;

        return new RfcError(RfcErrorInfo.Error(error.Message), error);
    }
}