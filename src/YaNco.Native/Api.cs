using System;
using System.Collections.Generic;

namespace Dbosoft.YaNco.Native
{
    public static class Api
    {

        public static ConnectionHandle OpenConnection(IDictionary<string, string> connectionParams,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();
        }

        public static FunctionDescriptionHandle GetFunctionDescription(FunctionHandle functionHandle,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static FunctionDescriptionHandle GetFunctionDescription(ConnectionHandle connectionHandle,
            string functionName, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetFunctionName(FunctionDescriptionHandle descriptionHandle, out string functionName,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static TypeDescriptionHandle GetTypeDescription(IDataContainerHandle dataContainer,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTypeFieldCount(TypeDescriptionHandle descriptionHandle, out int count,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, string name,
            out RfcFieldInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, int index,
            out RfcFieldInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static FunctionHandle CreateFunction(FunctionDescriptionHandle descriptionHandle,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetFunctionParameterCount(FunctionDescriptionHandle descriptionHandle, out int count,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle,
            string name, out RfcParameterInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle,
            int index, out RfcParameterInfo parameterInfo, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc Invoke(ConnectionHandle connectionHandle, FunctionHandle functionHandle,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetStructure(IDataContainerHandle dataContainer, string name,
            out StructureHandle structure, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTable(IDataContainerHandle dataContainer, string name, out TableHandle table,
            out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static TableHandle CloneTable(TableHandle tableHandle, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static void AllowStartOfPrograms(ConnectionHandle connectionHandle, StartProgramDelegate callback, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTableRowCount(TableHandle table, out int count, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static StructureHandle GetCurrentTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static StructureHandle AppendTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc MoveToNextTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc MoveToFirstTableRow(TableHandle table, out RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetInt(IDataContainerHandle containerHandle, string name, int value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetInt(IDataContainerHandle containerHandle, string name, out int value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetLong(IDataContainerHandle containerHandle, string name, long value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetLong(IDataContainerHandle containerHandle, string name, out long value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetDateString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetDateString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetTimeString(IDataContainerHandle containerHandle, string name, string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetTimeString(IDataContainerHandle containerHandle, string name, out string value, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, uint bufferLength, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

        public static RfcRc GetBytes(IDataContainerHandle containerHandle, string name, out byte[] buffer, out
            RfcErrorInfo errorInfo)
        {
            throw new NotImplementedException();

        }

    }
}