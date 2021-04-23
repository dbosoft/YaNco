using System;
using System.Collections.Generic;
using Dbosoft.YaNco.Internal;
using LanguageExt;
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
            if (result == null || errorInfo.Code != RfcRc.RFC_OK)
            {
                Logger.IfSome(l =>
                {
                    if (errorInfo.Code == RfcRc.RFC_OK)
                    {
                        if (logAsError)
                            l.LogError("received invalid null response from rfc call", result);
                        else
                            l.LogError("received invalid null response from rfc call", result);
                    }
                    else
                    {
                        if (logAsError)
                            l.LogError("received error from rfc call", errorInfo);
                        else
                            l.LogDebug("received error from rfc call", errorInfo);
                    }
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
            IConnectionHandle handle = Api.OpenConnection(connectionParams, out var errorInfo);
            return ResultOrError(handle, errorInfo, true);
        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
            string functionName)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function name", functionName));
            IFunctionDescriptionHandle handle = Api.GetFunctionDescription(connectionHandle as ConnectionHandle, functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function handle", functionHandle));
            IFunctionDescriptionHandle handle = Api.GetFunctionDescription(functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
        {
            Logger.IfSome(l => l.LogTrace("reading type description by container handle", dataContainer));
            ITypeDescriptionHandle handle = Api.GetTypeDescription(dataContainer as Internal.IDataContainerHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function name by description handle", descriptionHandle));
            var rc = Api.GetFunctionName(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading field count by type description handle", descriptionHandle));
            var rc = Api.GetTypeFieldCount(descriptionHandle as TypeDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            int index)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and index", new { descriptionHandle, index }));
            var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            string name)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and name", new { descriptionHandle, name }));
            var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("creating function by function description handle", descriptionHandle));
            IFunctionHandle handle = Api.CreateFunction(descriptionHandle as FunctionDescriptionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter count by function description handle", descriptionHandle));
            var rc = Api.GetFunctionParameterCount(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, int index)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and index", new { descriptionHandle, index }));
            var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and name", new { descriptionHandle, name }));
            var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("Invoking function", new { connectionHandle, functionHandle }));
            var rc = Api.Invoke(connectionHandle as ConnectionHandle, functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating structure by data container handle and name", new { dataContainer, name }));
            var rc = Api.GetStructure(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((IStructureHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating table by data container handle and name", new { dataContainer, name }));
            var rc = Api.GetTable(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((ITableHandle)result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, ITableHandle> CloneTable(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("cloning table by tableHandle", tableHandle));
            ITableHandle handle = Api.CloneTable(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle,
            StartProgramDelegate callback)
        {
            Logger.IfSome(l => l.LogTrace("Setting allow start of programs callback"));
            Api.AllowStartOfPrograms(connectionHandle as ConnectionHandle, callback, out var errorInfo);
            return ResultOrError(Unit.Default, errorInfo.Code, errorInfo);

        }

        public Either<RfcErrorInfo, int> GetTableRowCount(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading table row count by table handle", tableHandle));
            var rc = Api.GetTableRowCount(tableHandle as TableHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading current table row by table handle", tableHandle));
            IStructureHandle handle = Api.GetCurrentTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("append table row by table handle", tableHandle));
            IStructureHandle handle = Api.AppendTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToNextTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to next table row by table handle", tableHandle));
            var rc = Api.MoveToNextTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to first table row by table handle", tableHandle));
            var rc = Api.MoveToFirstTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting string value by name", new { containerHandle, name, value}));
            var rc = Api.SetString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading string value by name", new { containerHandle, name}));
            var rc = Api.GetString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting date string value by name", new { containerHandle, name, value }));
            var rc = Api.SetDateString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetDateString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading date string value by name", new { containerHandle, name }));
            var rc = Api.GetDateString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcErrorInfo, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting time string value by name", new { containerHandle, name, value }));
            var rc = Api.SetTimeString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, string> GetTimeString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting time string value by name", new { containerHandle, name }));
            var rc = Api.GetTimeString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Option<ILogger> Logger { get; }

        public Either<RfcErrorInfo, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value)
        {
            Logger.IfSome(l => l.LogTrace("setting int value by name", new { containerHandle, name, value }));
            var rc = Api.SetInt(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, int> GetInt(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting int value by name", new { containerHandle, name }));
            var rc = Api.GetInt(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
        {
            Logger.IfSome(l => l.LogTrace("setting long value by name", new { containerHandle, name, value }));
            var rc = Api.SetLong(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcErrorInfo, long> GetLong(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting long value by name", new { containerHandle, name }));
            var rc = Api.GetLong(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcErrorInfo, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
        {
            Logger.IfSome(l => l.LogTrace("setting byte value by name", new { containerHandle, name }));
            var rc = Api.SetBytes(containerHandle as Internal.IDataContainerHandle, name, buffer, (uint) bufferLength, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcErrorInfo, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting byte value by name", new { containerHandle, name }));
            var rc = Api.GetBytes(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }
    }
}