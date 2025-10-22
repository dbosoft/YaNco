using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco.Internal;

[ExcludeFromCodeCoverage]
internal static class Interopt
{
    private const string SapNwRfcName = "sapnwrfc";

    [DllImport(SapNwRfcName)]
    public static extern IntPtr RfcGetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetTraceDir(string traceDir, out RfcErrorInfo errorInfo);


    [DllImport(SapNwRfcName)]
    public static extern RfcRc RfcSetMaximumStoredTraceFiles(int numberOfFiles, out RfcErrorInfo errorInfo);


    [DllImport(SapNwRfcName)]
    public static extern RfcRc RfcSetCpicTraceLevel(uint traceLevel, out RfcErrorInfo errorInfo);


    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcLoadCryptoLibrary(string pathToLibrary, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetIniPath(string pathName, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName)]
    public static extern RfcRc RfcReloadIniFile(out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcOpenConnection(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcCloseConnection(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcIsConnectionHandleValid(IntPtr rfcHandle, out int isValid, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetConnectionAttributes(IntPtr rfcHandle, out RfcAttributes attributes, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcCancel(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcCreateServer(RfcConnectionParameter[] connectionParams, uint paramCount, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcDestroyServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcLaunchServer(IntPtr rfcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetServerContext(IntPtr rfcHandle, out RfcServerContext context, out RfcErrorInfo errorInfo);


    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcShutdownServer(IntPtr rfcHandle, uint timeout, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcAddServerStateChangedListener(IntPtr serverHandle, ServerStateChangedCallback listener, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcAddServerErrorListener(IntPtr serverHandle, ServerErrorCallback listener, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetServerAttributes(IntPtr serverHandle, out RfcServerStateAttributes attributes, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcGetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcDescribeFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetFunctionName(IntPtr funcDesc, out string funcName, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcDescribeType(IntPtr dataHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcGetTypeDesc(IntPtr rfcHandle, string typeName, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetFieldCount(IntPtr typeHandle, out uint count, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetFieldDescByIndex(IntPtr typeHandle, uint index, out RFC_FIELD_INFO fieldDescr,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetFieldDescByName(IntPtr typeHandle, string name, out RFC_FIELD_INFO fieldDescr,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcCreateFunction(IntPtr funcDesc, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetParameterCount(IntPtr funcDesc, out uint count, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetParameterDescByIndex(IntPtr funcDesc, uint index,
        out RFC_PARAMETER_DESC paramDesc, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetParameterDescByName(IntPtr funcDesc, string name,
        out RFC_PARAMETER_DESC paramDesc, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName)]
    public static extern RfcRc RfcInvoke(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcCreateStructure(IntPtr descriptionHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcDestroyStructure(IntPtr structureHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetStructure(IntPtr dataHandle, string name, out IntPtr structHandle,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetStructureFromCharBuffer(IntPtr structureHandle,
        char[] charBuffer, uint bufferLength, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetTable(IntPtr dataHandle, string name, out IntPtr tableHandle,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcCloneTable(IntPtr srcTableHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetRowCount(IntPtr tableHandle, out uint rowCount, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcGetCurrentRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcAppendNewRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcMoveToNextRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcMoveToFirstRow(IntPtr tableHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetDate(IntPtr dataHandle, string name, char[] emptyDate,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetDate(IntPtr dataHandle, string name, char[] date, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetTime(IntPtr dataHandle, string name, char[] emptyTime,
        out RfcErrorInfo errorInfo);


    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetTime(IntPtr dataHandle, string name, char[] time, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetXString(IntPtr dataHandle, string name, byte[] bytesBuffer, uint bufferLength,
        out uint xstringLength, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetBytes(IntPtr dataHandle, string name, byte[] value, uint valueLength,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetInt(IntPtr dataHandle, string name, out int value, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetInt(IntPtr dataHandle, string name, int value, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetInt8(IntPtr dataHandle, string name, out long value,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetInt8(IntPtr dataHandle, string name, long value, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetString(IntPtr dataHandle, string name, char[] stringBuffer, uint bufferLength,
        out uint stringLength, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcSetString(IntPtr dataHandle, string name, string value, uint valueLength,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern IntPtr RfcCreateFunctionDesc(string name, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcAddParameter(IntPtr funcDesc, ref RFC_PARAMETER_DESC paramDescr,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcInstallServerFunction(string sysId, IntPtr funcDescHandle, RfcServerFunction serverFunction, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcInstallTransactionHandlers(string sysId,
        TransactionEventCallback onCheckFunction,
        TransactionEventCallback onCommitFunction,
        TransactionEventCallback onRollbackFunction,
        TransactionEventCallback onConfirmFunction,
        out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcGetStringByIndex(IntPtr dataHandle, uint index, char[] stringBuffer,
        uint bufferLength, out uint stringLength, out RfcErrorInfo errorInfo);



    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcDestroyFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcDestroyFunctionDesc(IntPtr funcDesc, out RfcErrorInfo errorInfo);

    [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
    public static extern RfcRc RfcDestroyTable(IntPtr tableHandle, out RfcErrorInfo errorInfo);


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct RfcConnectionParameter
    {
        [MarshalAs(UnmanagedType.LPTStr)] public string Name;

        [MarshalAs(UnmanagedType.LPTStr)] public string Value;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    // ReSharper disable once InconsistentNaming
    internal struct RFC_FIELD_INFO
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string Name;

        [MarshalAs(UnmanagedType.I4)] public RfcType Type;

        [MarshalAs(UnmanagedType.U4)] public uint NucLength;

        [MarshalAs(UnmanagedType.U4)] public uint NucOffset;

        [MarshalAs(UnmanagedType.U4)] public uint UcLength;

        [MarshalAs(UnmanagedType.U4)] public uint UcOffset;

        [MarshalAs(UnmanagedType.U4)] public uint Decimals;

        public IntPtr TypeDescHandle;

        public IntPtr ExtendedDescription;

    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    // ReSharper disable once InconsistentNaming
    internal struct RFC_PARAMETER_DESC
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string Name;

        [MarshalAs(UnmanagedType.I4)]
        public RfcType Type;

        [MarshalAs(UnmanagedType.I4)]
        public RfcDirection Direction;

        [MarshalAs(UnmanagedType.U4)]
        public uint NucLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint UcLength;

        [MarshalAs(UnmanagedType.U4)]
        public uint Decimals;

        public IntPtr TypeDescHandle;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30 + 1)]
        public string DefaultValue;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 79 + 1)]
        public string ParameterText;
        public char Optional;

        public IntPtr ExtendedDescription;



    }

    internal enum RfcServerState
    {
        RFC_SERVER_INITIAL = 0,
        RFC_SERVER_STARTING = 1,
        RFC_SERVER_RUNNING = 2,
        RFC_SERVER_BROKEN = 3,
        RFC_SERVER_STOPPING = 4,
        RFC_SERVER_STOPPED = 5
    }

    internal enum RfcCallType
    {
        RFC_SYNCHRONOUS,                // It's a standard synchronous RFC call.
        RFC_TRANSACTIONAL,              // This function call is part of a transactional LUW (tRFC).
        RFC_QUEUED,                     // This function call is part of a queued LUW (qRFC).
        RFC_BACKGROUND_UNIT             // This function call is part of a background LUW (bgRFC).
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcUnitIdentifier
    {
        public char unitType;                    // 'T' for "transactional" behavior (unit is executed synchronously), 'Q' for "queued" behavior (unit is written into a queue and executed asynchronously)

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string unitID;                    // The 32 digit unit ID of the background unit.
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcUnitAttributes
    {
        public short KernelTrace;             // If != 0, the backend will write kernel traces, while executing this unit.
        public short SatTrace;                // If != 0, the backend will write statistic records, while executing this unit.
        public short UnitHistory;             // If != 0, the backend will keep a "history" for this unit.
        public short Lock;                    // Used only for type Q: If != 0, the unit will be written to the queue, but not processed.
        // The unit can then be started manually in the ABAP debugger.
        public short NoCommitCheck;           // default the backend will check during execution of a unit, whether one
        // of the unit's function modules triggers an explicit or implicit COMMIT WORK.
        // In this case the unit is aborted with an error, because the transactional
        // integrity of this unit cannot be guaranteed. By setting "noCommitCheck" to
        // true (!=0), this behavior can be suppressed, meaning the unit will be executed
        // anyway, even if one of its function modules "misbehaves" and triggers a COMMIT WORK.

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12 + 1)]
        public string User;        // Sender User (optional). Default is current operating system User.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3 + 1)]
        public string Client;       // Sender Client ("Mandant") (optional). Default is "000".
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20 + 1)]
        public string TCode;       // Sender Transaction Code (optional). Default is "".
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40 + 1)]
        public string Program;     // Sender Program (optional). Default is current executable name.
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40 + 1)]
        public string Hostname;    // Sender hostname. Used only when the external program is server. In the client case the nwrfclib fills this automatically.

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string SendingDate;       // Sending date in UTC (GMT-0). Used only when the external program is server. In the client case the nwrfclib fills this automatically.

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string SendingTime;       // Sending time in UTC (GMT-0). Used only when the external program is server. In the client case the nwrfclib fills this automatically.
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcServerContext
    {
        public RfcCallType Type;  // Specifies the type of function call. Depending on the value of this field, some of the other fields of this struct may be filled.

        //24 chars
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Tid;         // If type is RFC_TRANSACTIONAL or RFC_QUEUED, this field is filled with the 24 digit TID of the tRFC/qRFC unit.

        public RfcUnitIdentifier UnitIdentifier;    // If type is RFC_BACKGROUND_UNIT, this pointer is set to the unit identifier of the LUW. Note: the pointer is valid only during the execution context of your server function.
        public RfcUnitAttributes UnitAttributes;    // If type is RFC_BACKGROUND_UNIT, this pointer is set to the unit attributes of the LUW. Note: the pointer is valid only during the execution context of your server function.
        public uint IsStateful;       // Specifies whether the current server connection is processing stateful RFC requests (assigned permanently to one fixed ABAP user session).

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string SessionID;      // Contains a unique zero-terminated session ID, identifying the ABAP or external user session. Can be used in stateful servers to store session context in a hashmap.

        public RfcServerAttributes ToAttributes()
        {
            var callType = Type switch
            {
                RfcCallType.RFC_SYNCHRONOUS => YaNco.RfcCallType.SYNCHRONOUS,
                RfcCallType.RFC_TRANSACTIONAL => YaNco.RfcCallType.TRANSACTIONAL,
                RfcCallType.RFC_QUEUED => YaNco.RfcCallType.QUEUED,
                RfcCallType.RFC_BACKGROUND_UNIT => YaNco.RfcCallType.BACKGROUND_UNIT,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new RfcServerAttributes(Tid, callType);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct RfcServerStateChange
    {
        public RfcServerState OldState;
        public RfcServerState NewState;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcServerStateAttributes
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string ServerName;
        public RfcServerState State;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    internal struct RfcAttributes
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64 + 1)]
        public string Destination;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string Host;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100 + 1)]
        public string PartnerHost;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string SystemNumber;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string SystemId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3 + 1)]
        public string Client;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12 + 1)]
        public string User;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string Language;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Trace;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2 + 1)]
        public string IsoLanguage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string Codepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string RfcRole;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerType;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string SystemRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerSystemRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerKernelRelease;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8 + 1)]
        public string CpicConversionId;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128 + 1)]
        public string ProgramName;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1 + 1)]
        public string PartnerBytesPerChar;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4 + 1)]
        public string PartnerSystemCodepage;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15 + 1)]
        public string PartnerIPv4;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 45 + 1)]
        public string PartnerIPv6;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string Reserved;

        public ConnectionAttributes ToConnectionAttributes()
        {
            return new ConnectionAttributes(
                Destination,
                Host,
                PartnerHost,
                SystemNumber,
                SystemId,
                Client,
                User,
                Language,
                Trace,
                IsoLanguage,
                Codepage,
                PartnerCodepage,
                RfcRole,
                Type,
                PartnerType,
                SystemRelease,
                PartnerSystemRelease,
                PartnerKernelRelease,
                CpicConversionId,
                ProgramName,
                PartnerBytesPerChar,
                PartnerSystemCodepage,
                PartnerIPv4,
                PartnerIPv6);
        }
    }

    public delegate RfcRc RfcServerFunction(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    public delegate RfcRc TransactionEventCallback(IntPtr rfcHandle, string tid);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    public delegate void ServerStateChangedCallback(IntPtr serverHandle, ref RfcServerStateChange stateChange);

    [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
    public delegate void ServerErrorCallback(IntPtr serverHandle, IntPtr clientInfoPtr, ref RfcErrorInfo errorInfo);


}