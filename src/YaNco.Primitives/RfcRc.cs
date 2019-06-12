

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco
{
    /// <summary>
    /// <para>RFC return codes used by all functions that do not directly return a handle.</para>
    /// <para>Also used as error indicator in the structure #RFC_ERROR_INFO::code.</para>
    /// </summary>
    public enum RfcRc
    {
        /// <summary>Everything O.K. Used by every function</summary>
        RFC_OK = 0,
        /// <summary>Error in Network&amp;Communication layer</summary>
        RFC_COMMUNICATION_FAILURE = 1,
        /// <summary>Unable to logon to SAP system. Invalid password, user locked, etc.</summary>
        RFC_LOGON_FAILURE = 2,
        /// <summary>SAP system runtime error (SYSTEM_FAILURE): Shortdump on the backend side</summary>
        RFC_ABAP_RUNTIME_FAILURE = 3,
        /// <summary>The called function module raised an E-, A- or X-Message</summary>
        RFC_ABAP_MESSAGE = 4,
        /// <summary>The called function module raised an Exception (RAISE or MESSAGE ... RAISING)</summary>
        RFC_ABAP_EXCEPTION = 5,
        /// <summary>Connection closed by the other side</summary>
        RFC_CLOSED = 6,
        /// <summary>No longer used</summary>
        RFC_CANCELED = 7,
        /// <summary>Time out</summary>
        RFC_TIMEOUT = 8,
        /// <summary>Memory insufficient</summary>
        RFC_MEMORY_INSUFFICIENT = 9,
        /// <summary>Version mismatch</summary>
        RFC_VERSION_MISMATCH = 10,
        /// <summary>The received data has an unsupported format</summary>
        RFC_INVALID_PROTOCOL = 11,
        /// <summary>A problem while serializing or deserializing RFM parameters</summary>
        RFC_SERIALIZATION_FAILURE = 12,
        /// <summary>An invalid handle was passed to an API call</summary>
        RFC_INVALID_HANDLE = 13,
        /// <summary>RfcListenAndDispatch did not receive an RFC request during the timeout period</summary>
        RFC_RETRY = 14,
        /// <summary>Error in external custom code. (E.g. in the function handlers or tRFC handlers.) Results in SYSTEM_FAILURE</summary>
        RFC_EXTERNAL_FAILURE = 15,
        /// <summary>Inbound tRFC Call already executed (needs to be returned from RFC_ON_CHECK_TRANSACTION in case the TID is already known and successfully processed before.)</summary>
        RFC_EXECUTED = 16,
        /// <summary>Function or structure definition not found (Metadata API)</summary>
        RFC_NOT_FOUND = 17,
        /// <summary>The operation is not supported on that handle</summary>
        RFC_NOT_SUPPORTED = 18,
        /// <summary>The operation is not supported on that handle at the current point of time (e.g. trying a callback on a server handle, while not in a call)</summary>
        RFC_ILLEGAL_STATE = 19,
        /// <summary>An invalid parameter was passed to an API call, (e.g. invalid name, type or length)</summary>
        RFC_INVALID_PARAMETER = 20,
        /// <summary>Codepage conversion error</summary>
        RFC_CODEPAGE_CONVERSION_FAILURE = 21,
        /// <summary>Error while converting a parameter to the correct data type</summary>
        RFC_CONVERSION_FAILURE = 22,
        /// <summary>The given buffer was to small to hold the entire parameter. Data has been truncated.</summary>
        RFC_BUFFER_TOO_SMALL = 23,
        /// <summary>Trying to move the current position before the first row of the table</summary>
        RFC_TABLE_MOVE_BOF = 24,
        /// <summary>Trying to move the current position after the last row of the table</summary>
        RFC_TABLE_MOVE_EOF = 25,
        /// <summary>Failed to start and attach SAPGUI to the RFC connection</summary>
        RFC_START_SAPGUI_FAILURE = 26,
        /// <summary>The called function module raised a class based exception</summary>
        RFC_ABAP_CLASS_EXCEPTION = 27,
        /// <summary>&quot;Something&quot; went wrong, but I don't know what...</summary>
        RFC_UNKNOWN_ERROR = 28,
        /// <summary>Authorization check error</summary>
        RFC_AUTHORIZATION_FAILURE = 29,
        /// <summary>Don't use</summary>
        RFC_RC_maxValue = 30
    }


    /// <summary>Used in #RFC_PARAMETER_DESC::direction for specifying the direction of a function module parameter.</summary>
    public enum RfcDirection
    {
        /// <summary>Import parameter. This corresponds to ABAP IMPORTING parameter.</summary>
        Import = 1,
        /// <summary>Export parameter. This corresponds to ABAP EXPORTING parameter.</summary>
        Export = 2,
        /// <summary>Import and export parameter. This corresponds to ABAP CHANGING parameter.</summary>
        Changing = 3,
        /// <summary>Table parameter. This corresponds to ABAP TABLES parameter.</summary>
        Tables = 7
    }

    /// <summary>
    /// <para>RFCTYPE is used in field descriptions (#RFC_FIELD_DESC) and parameter descriptions</para>
    /// <para>(#RFC_PARAMETER_DESC) and denotes the ABAP data type of the corresponding field/parameter.</para>
    /// </summary>
    public enum RfcType
    {
        /// <summary>1-byte or multibyte character, fixed size, blank padded</summary>
        CHAR = 0,
        /// <summary>Date ( YYYYYMMDD )</summary>
        DATE = 1,
        /// <summary>Packed number, any length between 1 and 16 bytes</summary>
        BCD = 2,
        /// <summary>Time (HHMMSS)</summary>
        TIME = 3,
        /// <summary>Raw data, binary, fixed length, zero padded.</summary>
        BYTE = 4,
        /// <summary>Internal table</summary>
        TABLE = 5,
        /// <summary>Digits, fixed size, leading '0' padded.</summary>
        NUM = 6,
        /// <summary>Floating point, double precision</summary>
        FLOAT = 7,
        /// <summary>4-byte integer</summary>
        INT = 8,
        /// <summary>2-byte integer. Obsolete, not directly supported by ABAP/4</summary>
        INT2 = 9,
        /// <summary>1-byte integer, unsigned. Obsolete, not directly supported by ABAP/4</summary>
        INT1 = 10,
        /// <summary>Not supported data type.</summary>
        NULL = 14,
        /// <summary>ABAP object.</summary>
        ABAPOBJECT = 16,
        /// <summary>ABAP structure</summary>
        STRUCTURE = 17,
        /// <summary>IEEE 754r decimal floating point, 8 bytes</summary>
        DECF16 = 23,
        /// <summary>IEEE 754r decimal floating point, 16 bytes</summary>
        DECF34 = 24,
        /// <summary>No longer used!</summary>
        XMLDATA = 28,
        /// <summary>Variable-length, null-terminated string</summary>
        STRING = 29,
        /// <summary>Variable-length raw string, length in bytes</summary>
        XSTRING = 30,
        /// <summary>8-byte integer</summary>
        INT8 = 31,
        /// <summary>timestamp/long, 8-byte integer</summary>
        _UTCLONG = 32,
        /// <summary>timestamp/second, 8-byte integer</summary>
        UTCSECOND = 33,
        /// <summary>timestamp/minute, 8-byte integer</summary>
        UTCMINUTE = 34,
        /// <summary>date/day , 4-byte integer</summary>
        DTDAY = 35,
        /// <summary>date/week, 4-byte integer</summary>
        _DTWEEK = 36,
        /// <summary>date/month, 4-byte integer</summary>
        DTMONTH = 37,
        /// <summary>time/second, 4-byte integer</summary>
        TSECOND = 38,
        /// <summary>time/minute, 2-byte integer</summary>
        TMINUTE = 39,
        /// <summary>calendar day, 2-byte integer</summary>
        CDAY = 40,
        /// <summary>boxed structure, note: not supported by NW RFC lib</summary>
        BOX = 41,
        /// <summary>boxed client dependent structure, note: not supported by NW RFC lib</summary>
        GENERIC_BOX = 42,
        /// <summary>the max. value of RFCTYPEs</summary>
        //_maxValue = 43
    }
}