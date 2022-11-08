﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Dbosoft.YaNco.Internal
{
    [ExcludeFromCodeCoverage]
    internal static class Interopt
    {
        private const string SapNwRfcName = "sapnwrfc";

        [DllImport(SapNwRfcName)]
        public static extern RfcRc RfcGetVersion(out uint majorVersion, out uint minorVersion, out uint patchLevel);


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
        public static extern RfcRc RfcShutdownServer(IntPtr rfcHandle, uint timeout, out RfcErrorInfo errorInfo);

        [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
        public static extern IntPtr RfcGetFunctionDesc(IntPtr rfcHandle, string funcName, out RfcErrorInfo errorInfo);

        [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
        public static extern IntPtr RfcDescribeFunction(IntPtr funcHandle, out RfcErrorInfo errorInfo);

        [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
        public static extern RfcRc RfcGetFunctionName(IntPtr funcDesc, out string funcName, out RfcErrorInfo errorInfo);

        [DllImport(SapNwRfcName, CharSet = CharSet.Unicode)]
        public static extern IntPtr RfcDescribeType(IntPtr dataHandle, out RfcErrorInfo errorInfo);

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
        public static extern RfcRc RfcGetStructure(IntPtr dataHandle, string name, out IntPtr structHandle,
            out RfcErrorInfo errorInfo);

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
            public readonly string DefaultValue;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 79 + 1)]
            public readonly string ParameterText;
            public char Optional;

            public IntPtr ExtendedDescription;



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

    }
}