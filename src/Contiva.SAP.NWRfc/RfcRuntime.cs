using System.Collections.Generic;
using LanguageExt;
using NativeApi = Contiva.SAP.NWRfc.Api;
// ReSharper disable UnusedMember.Global

namespace Contiva.SAP.NWRfc
{
    public class RfcRuntime : IRfcRuntime
    {

        private static Either<RfcErrorInfo,TResult> ResultOrError<TResult>(TResult result, RfcErrorInfo errorInfo)
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

        public Either<RfcErrorInfo, ConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
        {
            var handle = NativeApi.OpenConnection(connectionParams, out var errorInfo);
            return ResultOrError(handle,errorInfo);
        }
        
        public Either<RfcErrorInfo, FunctionDescriptionHandle> GetFunctionDescription(ConnectionHandle connectionHandle,
            string functionName)
        {
            var handle = NativeApi.GetFunctionDescription(connectionHandle, functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, FunctionDescriptionHandle> GetFunctionDescription(FunctionHandle functionHandle)
        {
            var handle = NativeApi.GetFunctionDescription(functionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, TypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
        {
            var handle = NativeApi.GetTypeDescription(dataContainer, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetFunctionName(FunctionDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetFunctionName(descriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTypeFieldCount(TypeDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetTypeFieldCount(descriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle,
            int index)
        {
            var rc = NativeApi.GetTypeFieldDescription(descriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo,RfcFieldInfo> GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle,
            string name)
        {

           var rc = NativeApi.GetTypeFieldDescription(descriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, FunctionHandle> CreateFunction(FunctionDescriptionHandle descriptionHandle)
        {            
            var handle = NativeApi.CreateFunction(descriptionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetFunctionParameterCount(FunctionDescriptionHandle descriptionHandle)
        {
            var rc = NativeApi.GetFunctionParameterCount(descriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            FunctionDescriptionHandle descriptionHandle, int index)
        {
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            FunctionDescriptionHandle descriptionHandle, string name)
        {
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> Invoke(ConnectionHandle connectionHandle, FunctionHandle functionHandle)
        {
            var rc = NativeApi.Invoke(connectionHandle, functionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, StructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
        {
            var rc = NativeApi.GetStructure(dataContainer, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, TableHandle> GetTable(IDataContainerHandle dataContainer, string name)
        {
            var rc = NativeApi.GetTable(dataContainer, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, TableHandle> CloneTable(TableHandle tableHandle)
        {
            var handle = NativeApi.CloneTable(tableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> AllowStartOfPrograms(ConnectionHandle connectionHandle,
            StartProgramDelegate callback)
        {
            NativeApi.AllowStartOfPrograms(connectionHandle, callback, out var errorInfo);
            return ResultOrError(Unit.Default, errorInfo.Code, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTableRowCount(TableHandle tableHandle)
        {
            var rc = NativeApi.GetTableRowCount(tableHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, StructureHandle> GetCurrentTableRow(TableHandle tableHandle)
        {
            var handle = NativeApi.GetCurrentTableRow(tableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, StructureHandle> AppendTableRow(TableHandle tableHandle)
        {
            var handle = NativeApi.AppendTableRow(tableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToNextTableRow(TableHandle tableHandle)
        {
            var rc = NativeApi.MoveToNextTableRow(tableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToFirstTableRow(TableHandle tableHandle)
        {
            var rc = NativeApi.MoveToFirstTableRow(tableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            var rc = NativeApi.SetString(containerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name)
        {
            var rc = NativeApi.GetString(containerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }
    }
}