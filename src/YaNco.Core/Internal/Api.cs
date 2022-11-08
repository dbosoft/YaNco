using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LanguageExt;

namespace Dbosoft.YaNco.Internal
{
    [ExcludeFromCodeCoverage]
    public static class Api
    {
        
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
            var rc = Interopt.RfcGetFieldDescByName(descriptionHandle.Ptr, name.ToUpperInvariant(), out var parameterDescr, out errorInfo);
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
                Optional = parameterDescription.Optional ? 'X' : ' ',
                Decimals = parameterDescription.Decimals,
                NucLength = parameterDescription.NucLength,
                UcLength = parameterDescription.UcLength,
                TypeDescHandle = parameterDescription.TypeDescriptionHandle
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

        public static RfcRc CancelConnection(ConnectionHandle connectionHandle,
            out RfcErrorInfo errorInfo)
        {
            return Interopt.RfcCancel(connectionHandle.Ptr, out errorInfo);
        }

        public static RfcRc GetStructure(IDataContainerHandle dataContainer, string name,
            out StructureHandle structure, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcGetStructure(dataContainer.Ptr, name.ToUpperInvariant(), out var structPtr, out errorInfo);
            structure = structPtr == IntPtr.Zero ? null : new StructureHandle(structPtr);
            return rc;

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

        public static RfcRc RegisterServerFunctionHandler(string sysId, FunctionDescriptionHandle functionDescription,
            RfcFunctionDelegate functionHandler, out RfcErrorInfo errorInfo)
        {
            var rc = Interopt.RfcInstallServerFunction(sysId, functionDescription.Ptr, RFC_Function_Handler,
                out errorInfo);
            if (rc != RfcRc.RFC_OK)
            {
                return rc;
            }

            RegisteredFunctions.AddOrUpdate(functionDescription.Ptr, functionHandler, (c, v) => v);
            return rc;
        }

        private static readonly object AllowStartOfProgramsLock = new object();

        [Obsolete("Use method AllowStartOfPrograms of ConnectionBuilder. This method will be removed in next major release.")]
        public static void AllowStartOfPrograms(ConnectionHandle connectionHandle, StartProgramDelegate callback, out
            RfcErrorInfo errorInfo)
        {
            lock (AllowStartOfProgramsLock)
            {

                GetConnectionAttributes(connectionHandle, out var attributes, out errorInfo);
                if (errorInfo.Code != RfcRc.RFC_OK)
                    return;


                RfcErrorInfo errorInfoLocal = default;
                //function runtime is not available at this API level => as workaround create a new runtime -> in FunctionBuilder it is 
                //only used to wrap this API implementation.
                new FunctionBuilder(new RfcRuntime(), "RFC_START_PROGRAM")
                    .AddChar("COMMAND", RfcDirection.Import, 512)
                    .Build()
                    .Match(funcDescriptionHandle =>
                    {
                        RegisterServerFunctionHandler(attributes.SystemId, funcDescriptionHandle as FunctionDescriptionHandle,
                            (_, funcHandle) =>
                            {
                                var functionHandle = funcHandle as FunctionHandle;
                                Debug.Assert(functionHandle != null, nameof(functionHandle) + " != null");

                                var commandBuffer = new char[513];
                                var rc = Interopt.RfcGetStringByIndex(functionHandle.Ptr, 0, commandBuffer,
                                    (uint)commandBuffer.Length - 1, out var commandLength, out var error);

                                if (rc != RfcRc.RFC_OK)
                                    return Unit.Default;

                                var command = new string(commandBuffer, 0, (int)commandLength);
                                error = callback(command);

                                if (error.Code == RfcRc.RFC_OK)
                                    return Unit.Default;

                                return error;
                            }, out errorInfoLocal);
                    }, l => errorInfoLocal = l);

                errorInfo = errorInfoLocal;
            }

        }

        private static readonly ConcurrentDictionary<IntPtr, RfcFunctionDelegate> RegisteredFunctions
            = new ConcurrentDictionary<IntPtr, RfcFunctionDelegate>();

        private static RfcRc RFC_Function_Handler(IntPtr rfcHandle, IntPtr funcHandle, out RfcErrorInfo errorInfo)
        {
            var descriptionHandle = Interopt.RfcDescribeFunction(funcHandle, out errorInfo);
            if (descriptionHandle == IntPtr.Zero)
                return errorInfo.Code;


            if (!RegisteredFunctions.TryGetValue(descriptionHandle, out var functionDelegate))
            {
                Interopt.RfcGetFunctionName(descriptionHandle, out var funcName, out _);
                if (string.IsNullOrWhiteSpace(funcName))
                    funcName = "[unknown function]";

                errorInfo = new RfcErrorInfo(RfcRc.RFC_INVALID_HANDLE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, "",
                    $"no function handler registered for function '{funcName}'", "", "", "", "", "", "", "");
                return RfcRc.RFC_INVALID_HANDLE;

            }

            RfcErrorInfo errorInfoLocal = default;
            var rc = functionDelegate(new RfcHandle(rfcHandle), new FunctionHandle(funcHandle)).Match(
                Right: r => RfcRc.RFC_OK,
                l =>
                {
                    errorInfoLocal = l;
                    return l.Code;

                });

            errorInfo = errorInfoLocal;
            return rc;

        }

        [Obsolete("Callback handlers are no longer bound to connection. This method will do nothing and will be removed in next major release.")]
        // ReSharper disable once UnusedParameter.Global
        public static void RemoveCallbackHandler(IntPtr connectionHandle)
        {

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
}