namespace Dbosoft.SAP.NWRfc
{
    /// <summary>
    /// <para>Used in all functions of the NW RFC library to return detailed information about</para>
    /// <para>an error that has just occurred. This can be an error that the communication partner</para>
    /// <para>sent back to us, an error that occurred in the network layer or operating system,</para>
    /// <para>an internal error in the NW RFC library or an error that the application programmer</para>
    /// <para>(i.e. you) has committed...</para>
    /// </summary>
    /// <remarks>
    /// <para>Within a server function implementation, the application programmer (you) can return</para>
    /// <para>this structure to the RFC library in order to specify the error type&amp;message that</para>
    /// <para>you want to send back to the backend.</para>
    /// </remarks>
    public struct RfcErrorInfo
    {
        public readonly RfcRc Code;

        public readonly RfcErrorGroup Group;

        public readonly string Key;

        public readonly string Message;

        public readonly string AbapMsgClass;

        public readonly string AbapMsgType;

        public readonly string AbapMsgNumber;

        public readonly string AbapMsgV1;
        public readonly string AbapMsgV2;

        public readonly string AbapMsgV3;

        public readonly string AbapMsgV4;

        public RfcErrorInfo(RfcRc code, RfcErrorGroup @group, string key, string message, string abapMsgClass, string abapMsgType, string abapMsgNumber, string abapMsgV1, string abapMsgV2, string abapMsgV3, string abapMsgV4)
        {
            Code = code;
            Group = @group;
            Key = key;
            Message = message;
            AbapMsgClass = abapMsgClass;
            AbapMsgType = abapMsgType;
            AbapMsgNumber = abapMsgNumber;
            AbapMsgV1 = abapMsgV1;
            AbapMsgV2 = abapMsgV2;
            AbapMsgV3 = abapMsgV3;
            AbapMsgV4 = abapMsgV4;
        }

        public static RfcErrorInfo Ok()
        {
            return new RfcErrorInfo(RfcRc.RFC_OK, RfcErrorGroup.OK, "", "", "", "", "", "", "", "", "");
        }

        public static RfcErrorInfo EmptyResult()
        {
            return new RfcErrorInfo(RfcRc.RFC_ILLEGAL_STATE, RfcErrorGroup.EXTERNAL_RUNTIME_FAILURE, "unexpected empty result", "", "", "", "", "", "", "", "");
        }
    }
}