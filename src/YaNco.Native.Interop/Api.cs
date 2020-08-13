using System;
using System.Collections.Generic;
using System.Linq;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Dbosoft.YaNco.Native
{
    public static class Api
    {

        public static ConnectionHandle OpenConnection(IDictionary<string, string> connectionParams,
            out RfcErrorInfo errorInfo)
        {
            var rfcOptions = connectionParams.Select(x => new Interopt.RfcConnectionParameter { Name = x.Key, Value = x.Value })
                .ToArray();

            return new ConnectionHandle(Interopt.RfcOpenConnection(rfcOptions, (uint)rfcOptions.Length, out errorInfo));
        }

        public static FunctionDescriptionHandle GetFunctionDescription(FunctionHandle functionHandle,
            out RfcErrorInfo errorInfo)
        {
            return new FunctionDescriptionHandle(Interopt.RfcDescribeFunction(functionHandle.Ptr, out errorInfo));

        }

        public static FunctionDescriptionHandle GetFunctionDescription(ConnectionHandle connectionHandle,
            string functionName, out RfcErrorInfo errorInfo)
        {
            return new FunctionDescriptionHandle(Interopt.RfcGetFunctionDesc(connectionHandle.Ptr, functionName, out errorInfo));

        }

        public static RfcRc GetFunctionName(FunctionDescriptionHandle descriptionHandle, out string functionName,
            out RfcErrorInfo errorInfo)
        {
            return Interopt.RfcGetFunctionName(descriptionHandle.Ptr, out functionName, out errorInfo);

        }

        public static TypeDescriptionHandle GetTypeDescription(IDataContainerHandle dataContainer,
            out RfcErrorInfo errorInfo)
        {
            return new TypeDescriptionHandle(Interopt.RfcDescribeType(dataContainer.Ptr, out errorInfo));

        }

        public static RfcRc GetTypeFieldCount(TypeDescriptionHandle descriptionHandle, out int count,
            out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetFieldCount(descriptionHandle.Ptr, out var uIntCount, out errorInfo);
            count = (int)uIntCount;
            return rc;
        }

        public static RfcRc GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, string name,
            out RfcFieldInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetFieldDescByName(descriptionHandle.Ptr, name, out var parameterDescr, out errorInfo);
            parameterInfo = new RfcFieldInfo(parameterDescr.Name, parameterDescr.Type, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals);
            return rc;
        }

        public static RfcRc GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, int index,
            out RfcFieldInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetFieldDescByIndex(descriptionHandle.Ptr, (uint)index, out var parameterDescr, out errorInfo);
            parameterInfo = new RfcFieldInfo(parameterDescr.Name, parameterDescr.Type, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals);
            return rc;

        }

        public static FunctionHandle CreateFunction(FunctionDescriptionHandle descriptionHandle,
            out RfcErrorInfo errorInfo)
        {
            return new FunctionHandle(Interopt.RfcCreateFunction(descriptionHandle.Ptr, out errorInfo));

        }

        public static RfcRc GetFunctionParameterCount(FunctionDescriptionHandle descriptionHandle, out int count,
            out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetParameterCount(descriptionHandle.Ptr, out var uIntCount, out errorInfo);
            count = (int)uIntCount;
            return rc;

        }

        public static RfcRc GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle,
            string name, out RfcParameterInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetParameterDescByName(descriptionHandle.Ptr, name, out var parameterDescr, out errorInfo);
            parameterInfo = new RfcParameterInfo(
                parameterDescr.Name, parameterDescr.Type, parameterDescr.Direction, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals, parameterDescr.DefaultValue, parameterDescr.ParameterText, parameterDescr.Optional == 'X');
            return rc;

        }

        public static RfcRc GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle,
            int index, out RfcParameterInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetParameterDescByIndex(descriptionHandle.Ptr, (uint)index, out var parameterDescr, out errorInfo);
            parameterInfo = new RfcParameterInfo(
                parameterDescr.Name, parameterDescr.Type, parameterDescr.Direction, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals, parameterDescr.DefaultValue, parameterDescr.ParameterText, parameterDescr.Optional == 'X');
            return rc;

        }

        public static RfcRc Invoke(ConnectionHandle connectionHandle, FunctionHandle functionHandle,
            out RfcErrorInfo errorInfo)
        {
            return Interopt.RfcInvoke(connectionHandle.Ptr, functionHandle.Ptr, out errorInfo);

        }

        public static RfcRc GetStructure(IDataContainerHandle dataContainer, string name,
            out StructureHandle structure, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetStructure(dataContainer.Ptr, name, out var structPtr, out errorInfo);
            structure = new StructureHandle(structPtr);
            return rc;

        }

        public static RfcRc GetTable(IDataContainerHandle dataContainer, string name, out TableHandle table,
            out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetTable(dataContainer.Ptr, name, out var tablePtr, out errorInfo);
            table = new TableHandle(tablePtr);
            return rc;

        }

        public static TableHandle CloneTable(TableHandle tableHandle, out RfcErrorInfo errorInfo)
        {
            return new TableHandle(Interopt.RfcCloneTable(tableHandle.Ptr, out errorInfo));

        }

        public static void AllowStartOfPrograms(ConnectionHandle connectionHandle, StartProgramDelegate callback, out
            RfcErrorInfo errorInfo)
        {
            var descriptionHandle = new FunctionDescriptionHandle(Interopt.RfcCreateFunctionDesc("RFC_START_PROGRAM", out errorInfo));
            if (descriptionHandle.Ptr == IntPtr.Zero)
            {
                return;
            }

            var paramDesc = new Interopt.RFC_PARAMETER_DESC{ Name = "COMMAND", Type = RfcType.CHAR, Direction = RfcDirection.Import, NucLength = 512, UcLength = 1024};
            var rc = Interopt.RfcAddParameter(descriptionHandle.Ptr, ref paramDesc, out errorInfo);
            if (rc != RfcRc.RFC_OK)
            {
                return;
            }

            rc = Interopt.RfcInstallServerFunction(null, descriptionHandle.Ptr, RFC_START_PROGRAM_Handler, out errorInfo);
            if (rc != RfcRc.RFC_OK)
            {
                return;
            }

            StartProgramCallback = callback;

        }

        public static StartProgramDelegate StartProgramCallback;

        static RfcRc RFC_START_PROGRAM_Handler(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo)
        {
            var commandBuffer = new char[513];

            var rc = Interopt.RfcGetStringByIndex(funcHandle, 0, commandBuffer, (uint) commandBuffer.Length - 1, out var commandLength, out errorInfo);

            if (rc != RfcRc.RFC_OK)
                return rc;

            var command = new string(commandBuffer,0, (int) commandLength);
            errorInfo = StartProgramCallback(command);

            return errorInfo.Code;
        }

        public static RfcRc GetTableRowCount(TableHandle table, out int count, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetRowCount(table.Ptr, out var uIntCount, out errorInfo);
            count = (int)uIntCount;
            return rc;

        }

        public static StructureHandle GetCurrentTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            return new StructureHandle(Interopt.RfcGetCurrentRow(table.Ptr, out errorInfo));

        }

        public static StructureHandle AppendTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            return new StructureHandle(Interopt.RfcAppendNewRow(table.Ptr, out errorInfo));

        }

        public static RfcRc MoveToNextTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            return Interopt.RfcMoveToNextRow(table.Ptr, out errorInfo);

        }

        public static RfcRc MoveToFirstTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            return Interopt.MoveToFirstTableRow(table.Ptr, out errorInfo);

        }

        public static RfcRc SetString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetString(containerHandle.Ptr, name, value, (uint)value.Length, out errorInfo);

        }

        public static RfcRc GetString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            var buffer = new char[61];
            var rc = Interopt.RfcGetString(containerHandle.Ptr, name, buffer, 61, out var stringLength, out errorInfo);

            if (rc != RfcRc.RFC_BUFFER_TOO_SMALL)
            {
                value = new string(buffer, 0, (int)stringLength);
                return rc;
            }

            buffer = new char[stringLength + 1];
            rc = Interopt.RfcGetString(containerHandle.Ptr, name, buffer, stringLength + 1, out _, out errorInfo);
            value = new string(buffer, 0, (int)stringLength);
            return rc;
        }

        public static RfcRc SetInt(IDataContainerHandle containerHandle, string name, int value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetInt(containerHandle.Ptr, name, value, out errorInfo);

        }

        public static RfcRc GetInt(IDataContainerHandle containerHandle, string name, out int value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcGetInt(containerHandle.Ptr, name, out value, out errorInfo);

        }

        public static RfcRc SetLong(IDataContainerHandle containerHandle, string name, long value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetInt8(containerHandle.Ptr, name, value, out errorInfo);

        }

        public static RfcRc GetLong(IDataContainerHandle containerHandle, string name, out long value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcGetInt8(containerHandle.Ptr, name, out value, out errorInfo);

        }

        public static RfcRc SetDateString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetDate(containerHandle.Ptr, name, value.ToCharArray(0, 8), out errorInfo);

        }

        public static RfcRc GetDateString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            var buffer = new char[8];
            var rc = Interopt.RfcGetDate(containerHandle.Ptr, name, buffer, out errorInfo);
            value = new string(buffer);
            return rc;
        }

        public static RfcRc SetTimeString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetTime(containerHandle.Ptr, name, value.ToCharArray(0, 6), out errorInfo);

        }

        public static RfcRc GetTimeString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            var buffer = new char[6];
            var rc = Interopt.RfcGetTime(containerHandle.Ptr, name, buffer, out errorInfo);
            value = new string(buffer);
            return rc;
        }

        public static RfcRc SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, uint bufferLength, out
            RfcErrorInfo errorInfo)
        {
            return Interopt.RfcSetBytes(containerHandle.Ptr, name, buffer, bufferLength, out errorInfo);

        }

        public static RfcRc GetBytes(IDataContainerHandle containerHandle, string name, out byte[] buffer, out
            RfcErrorInfo errorInfo)
        {
            var tempBuffer = new byte[255];
            var rc = Interopt.RfcGetXString(containerHandle.Ptr, name, tempBuffer, 255, out var bufferLength, out errorInfo);

            if (rc != RfcRc.RFC_BUFFER_TOO_SMALL)
            {
                buffer = new byte[bufferLength];
                tempBuffer.CopyTo(buffer, 0);

                return rc;
            }

            tempBuffer = new byte[bufferLength];
            rc = Interopt.RfcGetXString(containerHandle.Ptr, name, tempBuffer, 255, out _, out errorInfo);
            buffer = new byte[bufferLength];
            tempBuffer.CopyTo(buffer, 0);

            return rc;
        }

    }
}