using System;
using System.Collections.Generic;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IRfcRuntime
    {
        IFieldMapper FieldMapper { get; }
        RfcRuntimeOptions Options { get; }
        Option<ILogger> Logger { get; }


        bool IsFunctionHandlerRegistered(string sysId, string functionName);
        Either<RfcErrorInfo, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams);
        Either<RfcErrorInfo, Unit> LaunchServer(IRfcServerHandle rfcServerHandle);
        Either<RfcErrorInfo, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout);
        Either<RfcErrorInfo, ServerContextAttributes> GetServerContext(IRfcHandle rfcHandle);
        Either<RfcErrorInfo, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams);
        Either<RfcErrorInfo, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName);
        Either<RfcErrorInfo, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription);

        Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
            string functionName);

        Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle);
        Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer);
        Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName);
        Either<RfcErrorInfo, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle);

        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            int index);

        Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            string name);

        Either<RfcErrorInfo, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle);
        Either<RfcErrorInfo, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle);

        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, int index);

        Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, string name);

        Either<RfcErrorInfo, Unit> AddFunctionHandler(string sysid, 
            string functionName,
            IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcErrorInfo, Unit>> handler);

        Either<RfcErrorInfo, Unit> AddTransactionHandlers(string sysid, 
            Func<IRfcHandle, string, RfcRc> onCheck,
            Func<IRfcHandle, string, RfcRc> onCommit,
            Func<IRfcHandle, string, RfcRc> onRollback,
            Func<IRfcHandle, string, RfcRc> onConfirm);

        Either<RfcErrorInfo, Unit> AddFunctionHandler(string sysid, 
            string functionName,
            IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcErrorInfo, Unit>> handler);

        Either<RfcErrorInfo, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle);
        Either<RfcErrorInfo, Unit> CancelConnection(IConnectionHandle connectionHandle);
        Either<RfcErrorInfo, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle);
        Either<RfcErrorInfo, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle);
        Either<RfcErrorInfo, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle);
        Either<RfcErrorInfo, Unit> SetStructure(IStructureHandle structureHandle, string content);
        Either<RfcErrorInfo, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name);
        Either<RfcErrorInfo, ITableHandle> CloneTable(ITableHandle tableHandle);

        Either<RfcErrorInfo, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle,
            StartProgramDelegate callback);

        Either<RfcErrorInfo, int> GetTableRowCount(ITableHandle tableHandle);
        Either<RfcErrorInfo, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, IStructureHandle> AppendTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, Unit> MoveToNextTableRow(ITableHandle tableHandle);
        Either<RfcErrorInfo, Unit> MoveToFirstTableRow(ITableHandle tableHandle);

        Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value);

        Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
            string value);

        Either<RfcErrorInfo, string> GetDateString(IDataContainerHandle containerHandle, string name);

        Either<RfcErrorInfo, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
            string value);

        Either<RfcErrorInfo, string> GetTimeString(IDataContainerHandle containerHandle, string name);
        Either<RfcErrorInfo, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcErrorInfo, RfcFieldInfo>> func);
        Either<RfcErrorInfo, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcErrorInfo, RfcFieldInfo>> func);
        Either<RfcErrorInfo, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value);
        Either<RfcErrorInfo, int> GetInt(IDataContainerHandle containerHandle, string name);
        Either<RfcErrorInfo, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value);
        Either<RfcErrorInfo, long> GetLong(IDataContainerHandle containerHandle, string name);
        Either<RfcErrorInfo, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength);
        Either<RfcErrorInfo, byte[]> GetBytes(IDataContainerHandle containerHandle, string name);
    }


}