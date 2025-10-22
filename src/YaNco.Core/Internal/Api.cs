using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace Dbosoft.YaNco.Internal;

[ExcludeFromCodeCoverage]
public static class Api
{
    public static Version GetVersion()
    {
        _ = Interopt.RfcGetVersion(out var major, out var minor, out var patch);
        return new Version((int) major, (int) minor, (int) patch);
    }

    public static RfcRc SetTraceDirectory(string traceDirectory, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetTraceDir(traceDirectory, out errorInfo);
    }
    public static RfcRc SetMaximumTraceFiles(int maxTraceFiles, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetMaximumStoredTraceFiles(maxTraceFiles, out errorInfo);
    }

    public static RfcRc SetCpicTraceLevel(int traceLevel, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetCpicTraceLevel((uint) traceLevel, out errorInfo);
    }

    public static RfcRc SetIniDirectory(string iniDirectory, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetIniPath(iniDirectory, out errorInfo);
    }

    public static RfcRc ReloadIniFile(out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcReloadIniFile(out errorInfo);
    }

    public static RfcRc LoadCryptoLibrary(string libraryPath, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcLoadCryptoLibrary(libraryPath, out errorInfo);
    }

    public static ConnectionHandle OpenConnection(IDictionary<string, string> connectionParams,
        out RfcErrorInfo errorInfo)
    {
        var rfcOptions = connectionParams.Select(x => 
                new Interopt.RfcConnectionParameter { Name = x.Key.ToUpperInvariant(), Value = x.Value })
            .ToArray();

        var ptr = Interopt.RfcOpenConnection(rfcOptions, (uint)rfcOptions.Length, out errorInfo);
        return ptr == IntPtr.Zero ? null : new ConnectionHandle(ptr);
    }

    public static RfcRc IsConnectionHandleValid(ConnectionHandle connectionHandle, out bool isValid, out RfcErrorInfo errorInfo)
    {
        if (connectionHandle.Ptr == IntPtr.Zero)
        {
            errorInfo = RfcErrorInfo.EmptyResult();
            isValid = false;
            return RfcRc.RFC_INVALID_HANDLE;
        }

        var rc = Interopt.RfcIsConnectionHandleValid(connectionHandle.Ptr, out var isValidInt, out errorInfo);
        isValid = isValidInt == 0;
        return rc;
    }

    public static RfcRc GetConnectionAttributes(ConnectionHandle connectionHandle, out ConnectionAttributes attributes, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetConnectionAttributes(connectionHandle.Ptr, out var rfcAttributes, out errorInfo);
        attributes = rfcAttributes.ToConnectionAttributes();
        return rc;
    }

    public static FunctionDescriptionHandle CreateFunctionDescription(string functionName,
        out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcCreateFunctionDesc(functionName, out errorInfo);
        return ptr == IntPtr.Zero ? null : new FunctionDescriptionHandle(ptr);
    }

    public static FunctionDescriptionHandle GetFunctionDescription(FunctionHandle functionHandle,
        out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcDescribeFunction(functionHandle.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new FunctionDescriptionHandle(ptr);

    }

    public static FunctionDescriptionHandle GetFunctionDescription(ConnectionHandle connectionHandle,
        string functionName, out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcGetFunctionDesc(connectionHandle.Ptr, functionName.ToUpperInvariant(), out errorInfo);
        return ptr == IntPtr.Zero ? null : new FunctionDescriptionHandle(ptr);
    }

    public static RfcRc GetFunctionName(FunctionDescriptionHandle descriptionHandle, out string functionName,
        out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcGetFunctionName(descriptionHandle.Ptr, out functionName, out errorInfo);

    }

    public static TypeDescriptionHandle GetTypeDescription(IDataContainerHandle dataContainer,
        out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcDescribeType(dataContainer.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new TypeDescriptionHandle(ptr);

    }

    [CanBeNull]
    public static TypeDescriptionHandle GetTypeDescription(ConnectionHandle connectionHandle, string typeName,
        out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcGetTypeDesc(connectionHandle.Ptr, typeName, out errorInfo);
        return ptr == IntPtr.Zero ? null : new TypeDescriptionHandle(ptr);

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
        var rc = Interopt.RfcGetFieldDescByName(descriptionHandle.Ptr, name.ToUpperInvariant(), out var parameterDescription, out errorInfo);
        parameterInfo = new RfcFieldInfo(parameterDescription.Name, parameterDescription.Type, parameterDescription.NucLength, parameterDescription.UcLength, parameterDescription.Decimals);
        return rc;
    }

    public static RfcRc GetTypeFieldDescription(TypeDescriptionHandle descriptionHandle, int index,
        out RfcFieldInfo parameterInfo, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetFieldDescByIndex(descriptionHandle.Ptr, (uint)index, out var parameterDescription, out errorInfo);
        parameterInfo = new RfcFieldInfo(parameterDescription.Name, parameterDescription.Type, parameterDescription.NucLength, parameterDescription.UcLength, parameterDescription.Decimals);
        return rc;

    }

    public static FunctionHandle CreateFunction(FunctionDescriptionHandle descriptionHandle,
        out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcCreateFunction(descriptionHandle.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new FunctionHandle(ptr);

    }

    public static RfcRc AddFunctionParameter(FunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription, out RfcErrorInfo errorInfo)
    {
        var parameterDesc = new Interopt.RFC_PARAMETER_DESC
        {
            Name = parameterDescription.Name,
            Type = parameterDescription.Type,
            Direction = parameterDescription.Direction,
            Optional = parameterDescription.Optional ? '1' : '0',
            Decimals = parameterDescription.Decimals,
            NucLength = parameterDescription.NucLength,
            UcLength = parameterDescription.UcLength,
            TypeDescHandle = parameterDescription.TypeDescriptionHandle,
            ExtendedDescription = IntPtr.Zero,
            ParameterText = parameterDescription.ParameterText ?? "",
            DefaultValue = parameterDescription.DefaultValue
        };

        return Interopt.RfcAddParameter(descriptionHandle.Ptr, ref parameterDesc, out errorInfo);
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
        var rc = Interopt.RfcGetParameterDescByName(descriptionHandle.Ptr, name.ToUpperInvariant(), out var parameterDescr, out errorInfo);
        parameterInfo = new RfcParameterInfo(
            parameterDescr.Name, parameterDescr.Type, parameterDescr.Direction, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals, parameterDescr.DefaultValue, parameterDescr.ParameterText, parameterDescr.Optional == 1);
        return rc;

    }

    public static RfcRc GetFunctionParameterDescription(FunctionDescriptionHandle descriptionHandle,
        int index, out RfcParameterInfo parameterInfo, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetParameterDescByIndex(descriptionHandle.Ptr, (uint)index, out var parameterDescr, out errorInfo);
        parameterInfo = new RfcParameterInfo(
            parameterDescr.Name, parameterDescr.Type, parameterDescr.Direction, parameterDescr.NucLength, parameterDescr.UcLength, parameterDescr.Decimals, parameterDescr.DefaultValue, parameterDescr.ParameterText, parameterDescr.Optional == 1);
        return rc;

    }

    public static RfcRc Invoke(ConnectionHandle connectionHandle, FunctionHandle functionHandle,
        out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcInvoke(connectionHandle.Ptr, functionHandle.Ptr, out errorInfo);

    }

    public static RfcRc CancelConnection(ConnectionHandle connectionHandle,
        out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcCancel(connectionHandle.Ptr, out errorInfo);
    }

    public static RfcRc LaunchServer(RfcServerHandle serverHandle, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcLaunchServer(serverHandle.Ptr, out errorInfo);
    }

    public static RfcRc ShutdownServer(RfcServerHandle serverHandle, int timeout, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcShutdownServer(serverHandle.Ptr, (uint)timeout, out errorInfo);
    }

    public static RfcServerHandle CreateServer(IDictionary<string, string> connectionParams,
        out RfcErrorInfo errorInfo)
    {
        var rfcOptions = connectionParams.Select(x => new Interopt.RfcConnectionParameter { Name = x.Key, Value = x.Value })
            .ToArray();

        var ptr = Interopt.RfcCreateServer(rfcOptions, (uint)rfcOptions.Length, out errorInfo);
        return ptr == IntPtr.Zero ? null : new RfcServerHandle(ptr);
    }

    public static RfcRc GetServerContext(RfcHandle rfcHandle, out RfcServerAttributes attributes, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetServerContext(rfcHandle.Ptr, out var rfcAttributes, out errorInfo);
        attributes = rfcAttributes.ToAttributes();
        return rc;
    }

    public static RfcRc GetStructure(IDataContainerHandle dataContainer, string name,
        out StructureHandle structure, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetStructure(dataContainer.Ptr, name.ToUpperInvariant(), out var structPtr, out errorInfo);
        structure = structPtr == IntPtr.Zero ? null : new StructureHandle(structPtr);
        return rc;

    }

    public static StructureHandle CreateStructure(TypeDescriptionHandle typeDescriptionHandle, out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcCreateStructure(typeDescriptionHandle.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new StructureHandle(ptr, true);
    }

    public static RfcRc SetStructure(StructureHandle structureHandle, string content, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetStructureFromCharBuffer(structureHandle.Ptr, content.ToCharArray(), (uint) content.Length,
            out errorInfo);
    }

    public static RfcRc GetTable(IDataContainerHandle dataContainer, string name, out TableHandle table,
        out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetTable(dataContainer.Ptr, name.ToUpperInvariant(), out var tablePtr, out errorInfo);
        table = tablePtr == IntPtr.Zero ? null : new TableHandle(tablePtr, false);
        return rc;

    }

    public static TableHandle CloneTable(TableHandle tableHandle, out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcCloneTable(tableHandle.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new TableHandle(ptr, true);
    }

    public static IDisposable RegisterTransactionFunctionHandlers(string sysId,
        Func<IRfcHandle, string, RfcRc> onCheck,
        Func<IRfcHandle, string, RfcRc> onCommit,
        Func<IRfcHandle, string, RfcRc> onRollback,
        Func<IRfcHandle, string, RfcRc> onConfirm, 
        out RfcErrorInfo errorInfo)
    {

        var holder = new TransactionEventHandlers(sysId, onCheck, onCommit, onRollback, onConfirm);


        _ = Interopt.RfcInstallTransactionHandlers(sysId,
            holder.OnCheck, holder.OnCommit, holder.OnRollback, holder.OnConfirm,
            out errorInfo);

        return holder;
            
    }
        
        
    public static IDisposable RegisterServerFunctionHandler(string sysId,
        FunctionDescriptionHandle functionDescription,
        RfcFunctionDelegate functionHandler, out RfcErrorInfo errorInfo)
    {
        var holder = new FunctionHandler(sysId, functionDescription, functionHandler);
        Interopt.RfcInstallServerFunction(sysId, functionDescription.Ptr, holder.ServerFunction,
            out errorInfo);
        return holder;
    }

    public static IDisposable RegisterServerListeners(
        RfcServerHandle serverHandle,
        Action<RfcServerStateChange> onStateChange,
        Action<ConnectionAttributes, RfcErrorInfo> onError,
        out RfcErrorInfo errorInfo)
    {
        var holder = new ServerEventListeners(serverHandle.Ptr, onStateChange, onError);

        RfcErrorInfo stateError = default;
        RfcErrorInfo errorListenerError = default;
        var stateRegistered = false;
        var errorRegistered = false;

        // Try to register state listener if provided
        if (onStateChange != null)
        {
            var rc = Interopt.RfcAddServerStateChangedListener(serverHandle.Ptr, holder.StateCallback, out stateError);
            stateRegistered = rc == RfcRc.RFC_OK;
        }

        // Try to register error listener if provided (independent of state listener result)
        if (onError != null)
        {
            var rc = Interopt.RfcAddServerErrorListener(serverHandle.Ptr, holder.ErrorCallback, out errorListenerError);
            errorRegistered = rc == RfcRc.RFC_OK;
        }

        // If at least one listener was requested but both failed, dispose and return error
        if ((onStateChange != null || onError != null) && !stateRegistered && !errorRegistered)
        {
            holder.Dispose();
            // Return the first error encountered
            errorInfo = onStateChange != null ? stateError : errorListenerError;
            return null;
        }

        errorInfo = default;
        return holder;
    }

    public static RfcRc GetTableRowCount(TableHandle table, out int count, out RfcErrorInfo errorInfo)
    {
        var rc = Interopt.RfcGetRowCount(table.Ptr, out var uIntCount, out errorInfo);
        count = (int)uIntCount;
        return rc;

    }

    public static StructureHandle GetCurrentTableRow(TableHandle table, out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcGetCurrentRow(table.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new StructureHandle(ptr);

    }

    public static StructureHandle AppendTableRow(TableHandle table, out RfcErrorInfo errorInfo)
    {
        var ptr = Interopt.RfcAppendNewRow(table.Ptr, out errorInfo);
        return ptr == IntPtr.Zero ? null : new StructureHandle(ptr);

    }

    public static RfcRc MoveToNextTableRow(TableHandle table, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcMoveToNextRow(table.Ptr, out errorInfo);

    }

    public static RfcRc MoveToFirstTableRow(TableHandle table, out RfcErrorInfo errorInfo)
    {
        return Interopt.RfcMoveToFirstRow(table.Ptr, out errorInfo);

    }

    public static RfcRc SetString(IDataContainerHandle containerHandle, string name, string value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetString(containerHandle.Ptr, name.ToUpperInvariant(), value, (uint)value.Length, out errorInfo);

    }

    public static RfcRc GetString(IDataContainerHandle containerHandle, string name, out string value, out
        RfcErrorInfo errorInfo)
    {
        var buffer = new char[61];
        var rc = Interopt.RfcGetString(containerHandle.Ptr, name.ToUpperInvariant(), buffer, 61, out var stringLength, out errorInfo);

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
        return Interopt.RfcSetInt(containerHandle.Ptr, name.ToUpperInvariant(), value, out errorInfo);

    }

    public static RfcRc GetInt(IDataContainerHandle containerHandle, string name, out int value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcGetInt(containerHandle.Ptr, name.ToUpperInvariant(), out value, out errorInfo);

    }

    public static RfcRc SetLong(IDataContainerHandle containerHandle, string name, long value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetInt8(containerHandle.Ptr, name.ToUpperInvariant(), value, out errorInfo);

    }

    public static RfcRc GetLong(IDataContainerHandle containerHandle, string name, out long value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcGetInt8(containerHandle.Ptr, name.ToUpperInvariant(), out value, out errorInfo);

    }

    public static RfcRc SetDateString(IDataContainerHandle containerHandle, string name, string value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetDate(containerHandle.Ptr, name.ToUpperInvariant(), value.ToCharArray(0, 8), out errorInfo);

    }

    public static RfcRc GetDateString(IDataContainerHandle containerHandle, string name, out string value, out
        RfcErrorInfo errorInfo)
    {
        var buffer = new char[8];
        var rc = Interopt.RfcGetDate(containerHandle.Ptr, name.ToUpperInvariant(), buffer, out errorInfo);
        value = new string(buffer);
        return rc;
    }

    public static RfcRc SetTimeString(IDataContainerHandle containerHandle, string name, string value, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetTime(containerHandle.Ptr, name.ToUpperInvariant(), value.ToCharArray(0, 6), out errorInfo);

    }

    public static RfcRc GetTimeString(IDataContainerHandle containerHandle, string name, out string value, out
        RfcErrorInfo errorInfo)
    {
        var buffer = new char[6];
        var rc = Interopt.RfcGetTime(containerHandle.Ptr, name.ToUpperInvariant(), buffer, out errorInfo);
        value = new string(buffer);
        return rc;
    }

    public static RfcRc SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, uint bufferLength, out
        RfcErrorInfo errorInfo)
    {
        return Interopt.RfcSetBytes(containerHandle.Ptr, name.ToUpperInvariant(), buffer, bufferLength, out errorInfo);

    }

    public static RfcRc GetBytes(IDataContainerHandle containerHandle, string name, out byte[] buffer, out
        RfcErrorInfo errorInfo)
    {
        var tempBuffer = new byte[255];
        var rc = Interopt.RfcGetXString(containerHandle.Ptr, name.ToUpperInvariant(), tempBuffer, 255, out var bufferLength, out errorInfo);

        if (rc != RfcRc.RFC_BUFFER_TOO_SMALL)
        {
            buffer = new byte[bufferLength];
            Array.Copy(tempBuffer, buffer, bufferLength);

            return rc;
        }

        tempBuffer = new byte[bufferLength];
        rc = Interopt.RfcGetXString(containerHandle.Ptr, name.ToUpperInvariant(), tempBuffer, bufferLength, out _, out errorInfo);
        buffer = new byte[bufferLength];
        tempBuffer.CopyTo(buffer, 0);

        return rc;
    }
        
}