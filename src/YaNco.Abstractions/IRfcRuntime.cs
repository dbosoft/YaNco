using System;
using System.Collections.Generic;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcRuntime
    {
        Either<RfcErrorInfo, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle, StartProgramDelegate callback);
        Either<RfcErrorInfo, IStructureHandle> AppendTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle, string functionName);
        Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle);
        Either<RfcErrorInfo, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(IFunctionDescriptionHandle descriptionHandle, int index);
        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(IFunctionDescriptionHandle descriptionHandle, string name);
        Either<RfcErrorInfo, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, ITableHandle> CloneTable(ITableHandle tableHandle);
        Either<RfcErrorInfo, int> GetTableRowCount(ITableHandle tableHandle);
        Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer);
        Either<RfcErrorInfo, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle, int index);
        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle, string name);
        Either<RfcErrorInfo, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle);
        Either<RfcErrorInfo, Unit> CancelConnection(IConnectionHandle connectionHandle);
        Either<RfcErrorInfo, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle);

        Either<RfcErrorInfo, Unit> MoveToNextTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, Unit> MoveToFirstTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams);
        Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name, string value);
        Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value);
        Either<RfcErrorInfo, int> GetInt(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value);
        Either<RfcErrorInfo, long> GetLong(IDataContainerHandle containerHandle, string name);
        Either<RfcErrorInfo, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength);
        Either<RfcErrorInfo, byte[]> GetBytes(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetDateString(IDataContainerHandle containerHandle, string name, string value);
        Either<RfcErrorInfo, string> GetDateString(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetTimeString(IDataContainerHandle containerHandle, string name, string value);
        Either<RfcErrorInfo, string> GetTimeString(IDataContainerHandle containerHandle, string name);

        Option<ILogger> Logger { get; }

        Either<RfcErrorInfo, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcErrorInfo, RfcFieldInfo>> func);
        Either<RfcErrorInfo, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcErrorInfo, RfcFieldInfo>> func);

        RfcRuntimeOptions Options { get; }
    }


}