using System.Collections.Generic;
using Dbosoft.YaNco.Native;
using LanguageExt;
using NativeApi = Dbosoft.YaNco.Native.Api;
// ReSharper disable UnusedMember.Global

namespace Dbosoft.YaNco
{
    public class RfcRuntime : IRfcRuntime
    {
        public RfcRuntime(ILogger logger = null)
        {
            Logger = logger == null ? Option<ILogger>.None : Option<ILogger>.Some(logger);
        }

        private Either<RfcErrorInfo, TResult> ResultOrError<TResult>(TResult result, RfcErrorInfo errorInfo, bool logAsError = false)
        {
            if (result == null)
            {
                Logger.IfSome(l =>
                {
                    if(logAsError)
                        l.LogError("received error from rfc call", errorInfo);
                    else
                        l.LogDebug("received error from rfc call", errorInfo);
                });
                return errorInfo;
            }

            Logger.IfSome(l => l.LogTrace("received result value from rfc call", result));

            return result;
        }

        private Either<RfcErrorInfo, TResult> ResultOrError<TResult>(TResult result, RfcRc rc, RfcErrorInfo errorInfo)
        {
            if (rc != RfcRc.RFC_OK)
            {
                Logger.IfSome(l => l.LogDebug("received error from rfc call", errorInfo));
                return errorInfo;
            }

            Logger.IfSome(l => l.LogTrace("received result value from rfc call", result));
            return result;
        }

        public Either<RfcErrorInfo, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
        {
            var loggedParams = new Dictionary<string,string>(connectionParams);

            // ReSharper disable StringLiteralTypo
            if (loggedParams.ContainsKey("passwd"))
                loggedParams["passwd"] = "XXXX";
            // ReSharper restore StringLiteralTypo

            Logger.IfSome(l => l.LogTrace("Opening connection", loggedParams));
            IConnectionHandle handle = NativeApi.OpenConnection(connectionParams, out var errorInfo);
            return ResultOrError(handle, errorInfo, true);
        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
            string functionName)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function name", functionName));
            IFunctionDescriptionHandle handle = NativeApi.GetFunctionDescription(connectionHandle as ConnectionHandle, functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function handle", functionHandle));
            IFunctionDescriptionHandle handle = NativeApi.GetFunctionDescription(functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
        {
            Logger.IfSome(l => l.LogTrace("reading type description by container handle", dataContainer));
            ITypeDescriptionHandle handle = NativeApi.GetTypeDescription(dataContainer as Native.IDataContainerHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function name by description handle", descriptionHandle));
            var rc = NativeApi.GetFunctionName(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading field count by type description handle", descriptionHandle));
            var rc = NativeApi.GetTypeFieldCount(descriptionHandle as TypeDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            int index)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and index", new { descriptionHandle, index }));
            var rc = NativeApi.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            string name)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and name", new { descriptionHandle, name }));
            var rc = NativeApi.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("creating function by function description handle", descriptionHandle));
            IFunctionHandle handle = NativeApi.CreateFunction(descriptionHandle as FunctionDescriptionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter count by function description handle", descriptionHandle));
            var rc = NativeApi.GetFunctionParameterCount(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, int index)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and index", new { descriptionHandle, index }));
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and name", new { descriptionHandle, name }));
            var rc = NativeApi.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("Invoking function", new { connectionHandle, functionHandle }));
            var rc = NativeApi.Invoke(connectionHandle as ConnectionHandle, functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating structure by data container handle and name", new { dataContainer, name }));
            var rc = NativeApi.GetStructure(dataContainer as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((IStructureHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating table by data container handle and name", new { dataContainer, name }));
            var rc = NativeApi.GetTable(dataContainer as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((ITableHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> CloneTable(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("cloning table by tableHandle", tableHandle));
            ITableHandle handle = NativeApi.CloneTable(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle,
            StartProgramDelegate callback)
        {
            Logger.IfSome(l => l.LogTrace("Setting allow start of programs callback"));
            NativeApi.AllowStartOfPrograms(connectionHandle as ConnectionHandle, callback, out var errorInfo);
            return ResultOrError(Unit.Default, errorInfo.Code, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTableRowCount(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading table row count by table handle", tableHandle));
            var rc = NativeApi.GetTableRowCount(tableHandle as TableHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading current table row by table handle", tableHandle));
            IStructureHandle handle = NativeApi.GetCurrentTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("append table row by table handle", tableHandle));
            IStructureHandle handle = NativeApi.AppendTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToNextTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to next table row by table handle", tableHandle));
            var rc = NativeApi.MoveToNextTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to first table row by table handle", tableHandle));
            var rc = NativeApi.MoveToFirstTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting string value by name", new { containerHandle, name, value}));
            var rc = NativeApi.SetString(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading string value by name", new { containerHandle, name}));
            var rc = NativeApi.GetString(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting date string value by name", new { containerHandle, name, value }));
            var rc = NativeApi.SetDateString(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetDateString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading date string value by name", new { containerHandle, name }));
            var rc = NativeApi.GetDateString(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting time string value by name", new { containerHandle, name, value }));
            var rc = NativeApi.SetTimeString(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetTimeString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting time string value by name", new { containerHandle, name }));
            var rc = NativeApi.GetTimeString(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Option<ILogger> Logger { get; }

        public Either<RfcErrorInfo, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value)
        {
            Logger.IfSome(l => l.LogTrace("setting int value by name", new { containerHandle, name, value }));
            var rc = NativeApi.SetInt(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, int> GetInt(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting int value by name", new { containerHandle, name }));
            var rc = NativeApi.GetInt(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
        {
            Logger.IfSome(l => l.LogTrace("setting long value by name", new { containerHandle, name, value }));
            var rc = NativeApi.SetLong(containerHandle as Native.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, long> GetLong(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting long value by name", new { containerHandle, name }));
            var rc = NativeApi.GetLong(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
        {
            Logger.IfSome(l => l.LogTrace("setting byte value by name", new { containerHandle, name }));
            var rc = NativeApi.SetBytes(containerHandle as Native.IDataContainerHandle, name, buffer, (uint) bufferLength, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting byte value by name", new { containerHandle, name }));
            var rc = NativeApi.GetBytes(containerHandle as Native.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }
    }
}