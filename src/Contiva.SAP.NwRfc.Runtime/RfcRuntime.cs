using System.Collections.Generic;
using Contiva.SAP.NWRfc.Native;
using LanguageExt;
using NativeApi = Contiva.SAP.NWRfc.Native.Api;
// ReSharper disable UnusedMember.Global

namespace Contiva.SAP.NWRfc
{
    public class RfcRuntime : IRfcRuntime
    {

        private static Either<RfcErrorInfo, TResult> ResultOrError<TResult>(TResult result, RfcErrorInfo errorInfo)
        {
            if (result == null)
                return errorInfo;

            return result;
        }

        private static Either<RfcErrorInfo, TResult> ResultOrError<TResult>(TResult result, RfcRc rc, RfcErrorInfo errorInfo)
        {
            if (rc != RfcRc.RFC_OK)
                return errorInfo;

            return result;
        }

        public Either<RfcErrorInfo, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
        {
            IConnectionHandle handle = NativeApi.OpenConnection(connectionParams, out var errorInfo);
            return ResultOrError(handle, errorInfo);
        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
            string functionName)
        {
            IFunctionDescriptionHandle handle = NativeApi.GetFunctionDescription(connectionHandle as ConnectionHandle, functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
        {
            IFunctionDescriptionHandle handle = NativeApi.GetFunctionDescription(functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
        {
            ITypeDescriptionHandle handle = NativeApi.GetTypeDescription(dataContainer as Native.IDataContainerHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetFunctionName(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetTypeFieldCount(descriptionHandle as TypeDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            int index)
        {
            var rc = NativeApi.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            string name)
        {

            var rc = NativeApi.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
        {
            IFunctionHandle handle = NativeApi.CreateFunction(descriptionHandle as FunctionDescriptionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetFunctionParameterCount(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, int index)
        {
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, string name)
        {
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
        {
            var rc = NativeApi.Invoke(connectionHandle as ConnectionHandle, functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
        {
            var rc = NativeApi.GetStructure(dataContainer as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((IStructureHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
        {
            var rc = NativeApi.GetTable(dataContainer as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((ITableHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> CloneTable(ITableHandle tableHandle)
        {
            ITableHandle handle = NativeApi.CloneTable(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle,
            StartProgramDelegate callback)
        {
            NativeApi.AllowStartOfPrograms(connectionHandle as ConnectionHandle, callback, out var errorInfo);
            return ResultOrError(Unit.Default, errorInfo.Code, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTableRowCount(ITableHandle tableHandle)
        {
            var rc = NativeApi.GetTableRowCount(tableHandle as TableHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
        {
            IStructureHandle handle = NativeApi.GetCurrentTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
        {
            IStructureHandle handle = NativeApi.AppendTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToNextTableRow(ITableHandle tableHandle)
        {
            var rc = NativeApi.MoveToNextTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
        {
            var rc = NativeApi.MoveToFirstTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            var rc = NativeApi.SetString(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name)
        {
            var rc = NativeApi.GetString(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value)
        {
            var rc = NativeApi.SetInt(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, int> GetInt(IDataContainerHandle containerHandle, string name)
        {
            var rc = NativeApi.GetInt(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
        {
            var rc = NativeApi.SetLong(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, long> GetLong(IDataContainerHandle containerHandle, string name)
        {
            var rc = NativeApi.GetLong(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
        {
            var rc = NativeApi.SetBytes(containerHandle as Native.IDataContainerHandle, name, buffer, (uint) bufferLength, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
        {

            var rc = NativeApi.GetBytes(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }
    }
}