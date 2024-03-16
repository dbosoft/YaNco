using System;
using System.Collections.Generic;
using Dbosoft.YaNco.TypeMapping;
using JetBrains.Annotations;
using LanguageExt;
// ReSharper disable InconsistentNaming

namespace Dbosoft.YaNco;

public interface SAPRfcDataIO: SAPRfcTypeIO, SAPRfcStructureIO, SAPRfcTableIO, SAPRfcFieldIO
{
}

public interface SAPRfcTypeIO
{
    Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer);
    Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName);
    Either<RfcError, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle);

    Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        int index);

    Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        string name);


}

public interface SAPRfcStructureIO
{
    Either<RfcError, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name);
    Either<RfcError, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle);
    Either<RfcError, Unit> SetStructure(IStructureHandle structureHandle, string content);

}

public interface SAPRfcTableIO
{
    RfcRuntimeOptions Options { get; }

    Either<RfcError, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name);
    Either<RfcError, ITableHandle> CloneTable(ITableHandle tableHandle);

    Either<RfcError, int> GetTableRowCount(ITableHandle tableHandle);
    Either<RfcError, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle);
    Either<RfcError, IStructureHandle> AppendTableRow(ITableHandle tableHandle);
    Either<RfcError, Unit> MoveToNextTableRow(ITableHandle tableHandle);
    Either<RfcError, Unit> MoveToFirstTableRow(ITableHandle tableHandle);

}

public interface SAPRfcFunctionIO
{
    Either<RfcError, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle);

    Either<RfcError, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle);

    Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, int index);

    Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
        IFunctionDescriptionHandle descriptionHandle, string name);

    Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler);

    Either<RfcError, IDisposable> AddFunctionHandler(string sysid,
        IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler);

    Either<RfcError, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle);
    Either<RfcError, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName);
    Either<RfcError, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription);

    Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
        string functionName);

    Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle);
    Either<RfcError, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle);

}

public interface SAPRfcConnectionIO
{
    Either<RfcError, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams);
    Either<RfcError, Unit> CancelConnection(IConnectionHandle connectionHandle);
    Either<RfcError, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle);
    Either<RfcError, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle);

}

public interface SAPRfcServerIO
{
    Either<RfcError, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams);
    Either<RfcError, Unit> LaunchServer(IRfcServerHandle rfcServerHandle);
    Either<RfcError, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout);
    Either<RfcError, RfcServerAttributes> GetServerCallContext(IRfcHandle rfcHandle);

    Either<RfcError, IDisposable> AddTransactionHandlers(string sysid,
        Func<IRfcHandle, string, RfcRc> onCheck,
        Func<IRfcHandle, string, RfcRc> onCommit,
        Func<IRfcHandle, string, RfcRc> onRollback,
        Func<IRfcHandle, string, RfcRc> onConfirm);


}

public interface SAPRfcFieldIO
{
    Either<RfcError, Unit> SetString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetString(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetDateString(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetTimeString(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value);
    Either<RfcError, int> GetInt(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value);
    Either<RfcError, long> GetLong(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength);
    Either<RfcError, byte[]> GetBytes(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func);
    Either<RfcError, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func);

    Either<RfcError, T> GetValue<T>(AbapValue abapValue);
    Either<RfcError, AbapValue> SetValue<T>(T value, RfcFieldInfo fieldInfo);

}


[Obsolete(Deprecations.RfcRuntime)]
[PublicAPI]
public interface IRfcRuntime : 
    SAPRfcServerIO, SAPRfcConnectionIO, SAPRfcFunctionIO, SAPRfcDataIO
{
    IFieldMapper FieldMapper { get; }
    Option<ILogger> Logger { get; }


}