using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.Live;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

// ReSharper disable UnusedMember.Global

namespace Dbosoft.YaNco;

[ExcludeFromCodeCoverage]
[Obsolete(Deprecations.RfcRuntime)]
public class RfcRuntime : RfcRuntime<SAPRfcRuntime>
{
    public RfcRuntime(SAPRfcRuntime runtime) : base(runtime)
    {
    }
}

[ExcludeFromCodeCoverage]
[Obsolete(Deprecations.RfcRuntime)]
public class RfcRuntime<RT> : IRfcRuntime
    where RT : struct, 
    HasSAPRfcServer<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, 
    HasSAPRfcLogger<RT>, HasSAPRfcData<RT>,
    HasEnvRuntimeSettings
{
    private readonly RT _runtime;

    public RfcRuntime(RT runtime)
    {
        _runtime = runtime;
    }

    public RfcRuntimeOptions Options => _runtime.Env.Settings.TableOptions;
    public IFieldMapper FieldMapper => _runtime.Env.Settings.FieldMapper;
    public Option<ILogger> Logger => _runtime.Env.Settings.Logger != null ? Prelude.Some(_runtime.Env.Settings.Logger) : Option<ILogger>.None;

    public Either<RfcError, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
    {
        return _runtime.RfcDataEff.Bind(io =>io.GetTable(dataContainer, name).ToEff(l=>l)).ToEither(_runtime);
    }

    public Either<RfcError, ITableHandle> CloneTable(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.CloneTable(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, int> GetTableRowCount(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTableRowCount(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetCurrentTableRow(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.AppendTableRow(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> MoveToNextTableRow(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.MoveToNextTableRow(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.MoveToFirstTableRow(tableHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams)
    {
        return _runtime.RfcServerEff.Bind(io => io.CreateServer(connectionParams).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> LaunchServer(IRfcServerHandle rfcServerHandle)
    {
        return _runtime.RfcServerEff.Bind(io => io.LaunchServer(rfcServerHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout)
    {
        return _runtime.RfcServerEff.Bind(io => io.ShutdownServer(rfcServerHandle, timeout).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, RfcServerAttributes> GetServerCallContext(IRfcHandle rfcHandle)
    {
        return _runtime.RfcServerEff.Bind(io => io.GetServerCallContext(rfcHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IDisposable> AddTransactionHandlers(string sysid, Func<IRfcHandle, string, RfcRc> onCheck, Func<IRfcHandle, string, RfcRc> onCommit, Func<IRfcHandle, string, RfcRc> onRollback, Func<IRfcHandle, string, RfcRc> onConfirm)
    {
        return _runtime.RfcServerEff.Bind(io => io.AddTransactionHandlers(sysid, onCheck, onCommit, onRollback, onConfirm).ToEff(l => l)).ToEither(_runtime);
    }


    public Either<RfcError, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.CreateFunction(descriptionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionParameterCount(descriptionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(IFunctionDescriptionHandle descriptionHandle, int index)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionParameterDescription(descriptionHandle, index).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(IFunctionDescriptionHandle descriptionHandle, string name)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionParameterDescription(descriptionHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IDisposable> AddFunctionHandler(string sysid, IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.AddFunctionHandler(sysid, function, handler).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IDisposable> AddFunctionHandler(string sysid, IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.AddFunctionHandler(sysid, descriptionHandle, handler).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.Invoke(connectionHandle, functionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.CreateFunctionDescription(functionName).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.AddFunctionParameter(descriptionHandle, parameterDescription).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle, string functionName)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionDescription(connectionHandle, functionName).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionDescription(functionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
    {
        return _runtime.RfcFunctionsEff.Bind(io => io.GetFunctionName(descriptionHandle).ToEff(l => l)).ToEither(_runtime);
    }


    public Either<RfcError, Unit> SetString(IDataContainerHandle containerHandle, string name, string value)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetString(containerHandle, name, value).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, string> GetString(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetString(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetDateString(IDataContainerHandle containerHandle, string name, string value)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetDateString(containerHandle, name, value).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, string> GetDateString(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetDateString(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetTimeString(IDataContainerHandle containerHandle, string name, string value)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetTimeString(containerHandle, name, value).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, string> GetTimeString(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTimeString(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetInt(containerHandle, name, value).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, int> GetInt(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetInt(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetLong(containerHandle, name, value).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, long> GetLong(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetLong(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetBytes(containerHandle, name, buffer, bufferLength).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetBytes(containerHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetFieldValue(handle, value, func).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetFieldValue<T>(handle, func).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, T> GetValue<T>(AbapValue abapValue)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetValue<T>(abapValue).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, AbapValue> SetValue<T>(T value, RfcFieldInfo fieldInfo)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetValue(value, fieldInfo).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTypeDescription(dataContainer).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTypeDescription(connectionHandle, typeName).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTypeFieldCount(descriptionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle, int index)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTypeFieldDescription(descriptionHandle, index).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetTypeFieldDescription(descriptionHandle, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
    {
        return _runtime.RfcDataEff.Bind(io => io.GetStructure(dataContainer, name).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle)
    {
        return _runtime.RfcDataEff.Bind(io => io.CreateStructure(typeDescriptionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> SetStructure(IStructureHandle structureHandle, string content)
    {
        return _runtime.RfcDataEff.Bind(io => io.SetStructure(structureHandle, content).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
    {
        return _runtime.RfcConnectionEff.Bind(io => io.OpenConnection(connectionParams).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, Unit> CancelConnection(IConnectionHandle connectionHandle)
    {
        return _runtime.RfcConnectionEff.Bind(io => io.CancelConnection(connectionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle)
    {
        return _runtime.RfcConnectionEff.Bind(io => io.IsConnectionHandleValid(connectionHandle).ToEff(l => l)).ToEither(_runtime);
    }

    public Either<RfcError, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle)
    {
        return _runtime.RfcConnectionEff.Bind(io => io.GetConnectionAttributes(connectionHandle).ToEff(l => l)).ToEither(_runtime);
    }

}