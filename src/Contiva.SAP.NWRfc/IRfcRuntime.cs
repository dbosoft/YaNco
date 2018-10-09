using System.Collections.Generic;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public interface IRfcRuntime
    {
        Either<RfcErrorInfo, Unit> AllowStartOfPrograms(ConnectionHandle connectionHandle, StartProgramDelegate callback);
        Either<RfcErrorInfo, StructureHandle> AppendTableRow(TableHandle tableHandle);
        Either<RfcErrorInfo, FunctionHandle> CreateFunction(FunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, StructureHandle> GetCurrentTableRow(TableHandle tableHandle);
        Either<RfcErrorInfo, FunctionDescriptionHandle> GetFunctionDescription(ConnectionHandle connectionHandle, string functionName);
        Either<RfcErrorInfo, FunctionDescriptionHandle> GetFunctionDescription(FunctionHandle functionHandle);
        Either<RfcErrorInfo, string> GetFunctionName(FunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, int> GetFunctionParameterCount(FunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle, int index);
        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle, string name);
        Either<RfcErrorInfo, StructureHandle> GetStructure(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, TableHandle> GetTable(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, TableHandle> CloneTable(TableHandle tableHandle);
        Either<RfcErrorInfo, int> GetTableRowCount(TableHandle tableHandle);
        Either<RfcErrorInfo, TypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer);
        Either<RfcErrorInfo, int> GetTypeFieldCount(TypeDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, int index);
        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, string name);
        Either<RfcErrorInfo, Unit> Invoke(ConnectionHandle connectionHandle, FunctionHandle functionHandle);
        Either<RfcErrorInfo, Unit> MoveToNextTableRow(TableHandle tableHandle);
        Either<RfcErrorInfo, Unit> MoveToFirstTableRow(TableHandle tableHandle);
        Either<RfcErrorInfo, ConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams);
        Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name, string value);
        Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name);
    }
}