using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco.Internal;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;
using FieldMappingContext = Dbosoft.YaNco.TypeMapping.FieldMappingContext;

// ReSharper disable UnusedMember.Global

namespace Dbosoft.YaNco
{
    [ExcludeFromCodeCoverage]
    public class RfcRuntime : IRfcRuntime
    {
        public IFieldMapper FieldMapper { get; }

        public RfcRuntime(ILogger logger = null, IFieldMapper fieldMapper = null, RfcRuntimeOptions options = null)
        {
            Logger = logger == null ? Option<ILogger>.None : Option<ILogger>.Some(logger);
            FieldMapper = fieldMapper ?? CreateDefaultFieldMapper();
            Options = options ?? new RfcRuntimeOptions();
        }

        public static IFieldMapper CreateDefaultFieldMapper(IEnumerable<Type> fromRfcConverters = null,
            IEnumerable<Type> toRfcConverters = null)
        {
            return new DefaultFieldMapper(
                new CachingConverterResolver(
                    DefaultConverterResolver.CreateWithBuildInConverters(fromRfcConverters, toRfcConverters)));
        }

        public RfcRuntimeOptions Options { get; }


        public Either<RfcError, IRfcServerHandle> CreateServer(IDictionary<string, string> connectionParams)
        {
            if (connectionParams.Count == 0)
                return new RfcError(new RfcErrorInfo(RfcRc.RFC_EXTERNAL_FAILURE,
                    RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, RfcRc.RFC_EXTERNAL_FAILURE.ToString(),
                    "Cannot open SAP connection with empty connection settings.",
                    "", "", "", "", "", "", ""));

            var loggedParams = new Dictionary<string, string>(connectionParams);

            Logger.IfSome(l => l.LogTrace("creating rfc server", loggedParams));
            IRfcServerHandle handle = Api.CreateServer(connectionParams, out var errorInfo);
            return ResultOrError(handle, errorInfo, true);

        }

        public Either<RfcError, Unit> LaunchServer(IRfcServerHandle rfcServerHandle)
        {
            Logger.IfSome(l => l.LogTrace("starting rfc server", rfcServerHandle));
            var rc = Api.LaunchServer(rfcServerHandle as RfcServerHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, Unit> ShutdownServer(IRfcServerHandle rfcServerHandle, int timeout)
        {
            Logger.IfSome(l => l.LogTrace($"stopping rfc server with timeout of {timeout} seconds.", rfcServerHandle));
            var rc = Api.ShutdownServer(rfcServerHandle as RfcServerHandle, timeout, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, RfcServerAttributes> GetServerCallContext(IRfcHandle rfcHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading server context", new { rfcHandle }));
            var rc = Api.GetServerContext(rfcHandle as RfcHandle, out var attributes, out var errorInfo);
            return ResultOrError(attributes, rc, errorInfo);
        }

        private Either<RfcError, TResult> ResultOrError<TResult>(TResult result, RfcErrorInfo errorInfo, bool logAsError = false)
        {
            if (result == null || errorInfo.Code != RfcRc.RFC_OK)
            {
                Logger.IfSome(l =>
                {
                    if (errorInfo.Code == RfcRc.RFC_OK)
                    {
                        if (logAsError)
                            l.LogError("received null result from api call.");
                        else
                            l.LogDebug("received null result from api call.");
                    }

                    if (logAsError)
                        l.LogError("received error from rfc call", errorInfo);
                    else
                        l.LogDebug("received error from rfc call", errorInfo);
                });
                return errorInfo.ToRfcError();
            }

            Logger.IfSome(l => l.LogTrace("received result value from rfc call", result));

            return result;
        }

        private Either<RfcError, TResult> ResultOrError<TResult>(TResult result, RfcRc rc, RfcErrorInfo errorInfo)
        {
            if (rc != RfcRc.RFC_OK)
            {
                Logger.IfSome(l => l.LogDebug("received error from rfc call", errorInfo));
                return errorInfo.ToRfcError();
            }

            Logger.IfSome(l => l.LogTrace("received result value from rfc call", result));
            return result;
        }

        public Either<RfcError, IConnectionHandle> OpenConnection(IDictionary<string, string> connectionParams)
        {
            if (connectionParams.Count == 0)
                return new RfcErrorInfo(RfcRc.RFC_EXTERNAL_FAILURE,
                    RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, RfcRc.RFC_EXTERNAL_FAILURE.ToString(),
                    "Cannot open SAP connection with empty connection settings.",
                    "", "", "", "", "", "", "").ToRfcError();

            var loggedParams = new Dictionary<string,string>(connectionParams);

            // ReSharper disable StringLiteralTypo
            if (loggedParams.ContainsKey("passwd"))
                loggedParams["passwd"] = "XXXX";
            // ReSharper restore StringLiteralTypo

            Logger.IfSome(l => l.LogTrace("Opening connection", loggedParams));
            IConnectionHandle handle = Api.OpenConnection(connectionParams, out var errorInfo);
            return ResultOrError(handle, errorInfo, true);
        }
        public Either<RfcError, IFunctionDescriptionHandle> CreateFunctionDescription(string functionName)
        {
            Logger.IfSome(l => l.LogTrace("creating function description without connection", functionName));
            IFunctionDescriptionHandle handle = Api.CreateFunctionDescription(functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);
        }

        public Either<RfcError, IFunctionDescriptionHandle> AddFunctionParameter(IFunctionDescriptionHandle descriptionHandle, RfcParameterDescription parameterDescription)
        {
            Logger.IfSome(l => l.LogTrace("adding parameter to function description", new { handle = descriptionHandle, parameter = parameterDescription }));
            var rc = Api.AddFunctionParameter(descriptionHandle as FunctionDescriptionHandle, parameterDescription, out var errorInfo);
            return ResultOrError(descriptionHandle, rc, errorInfo);
        }

        public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IConnectionHandle connectionHandle,
            string functionName)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function name", functionName));
            IFunctionDescriptionHandle handle = Api.GetFunctionDescription(connectionHandle as ConnectionHandle, functionName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, IFunctionDescriptionHandle> GetFunctionDescription(IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function description by function handle", functionHandle));
            IFunctionDescriptionHandle handle = Api.GetFunctionDescription(functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
        {
            Logger.IfSome(l => l.LogTrace("reading type description by container handle", dataContainer));
            ITypeDescriptionHandle handle = Api.GetTypeDescription(dataContainer as Internal.IDataContainerHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName)
        {
            Logger.IfSome(l => l.LogTrace("reading type description by type name", typeName));
            ITypeDescriptionHandle handle = Api.GetTypeDescription(connectionHandle as ConnectionHandle, typeName, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, string> GetFunctionName(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function name by description handle", descriptionHandle));
            var rc = Api.GetFunctionName(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading field count by type description handle", descriptionHandle));
            var rc = Api.GetTypeFieldCount(descriptionHandle as TypeDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            int index)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and index", new { descriptionHandle, index }));
            var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
            string name)
        {
            Logger.IfSome(l => l.LogTrace("reading field description by type description handle and name", new { descriptionHandle, name }));
            var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, IFunctionHandle> CreateFunction(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("creating function by function description handle", descriptionHandle));
            IFunctionHandle handle = Api.CreateFunction(descriptionHandle as FunctionDescriptionHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, int> GetFunctionParameterCount(IFunctionDescriptionHandle descriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter count by function description handle", descriptionHandle));
            var rc = Api.GetFunctionParameterCount(descriptionHandle as FunctionDescriptionHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, int index)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and index", new { descriptionHandle, index }));
            var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, index, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, RfcParameterInfo> GetFunctionParameterDescription(
            IFunctionDescriptionHandle descriptionHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading function parameter description by function description handle and name", new { descriptionHandle, name }));
            var rc = Api.GetFunctionParameterDescription(descriptionHandle as FunctionDescriptionHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, IDisposable> AddFunctionHandler(string sysid, 
            IFunction function, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
        {
            return GetFunctionDescription(function.Handle)
                .Use(used => used.Bind(d => AddFunctionHandler(sysid,
                    d, handler)));
        }

        public Either<RfcError, IDisposable> AddTransactionHandlers(string sysid, 
            Func<IRfcHandle, string, RfcRc> onCheck,
            Func<IRfcHandle, string, RfcRc> onCommit,
            Func<IRfcHandle, string, RfcRc> onRollback,
            Func<IRfcHandle, string, RfcRc> onConfirm)
        {
            var holder = Api.RegisterTransactionFunctionHandlers(sysid,
                  onCheck, onCommit, onRollback, onConfirm,
                out var errorInfo);

            return ResultOrError(holder, errorInfo);
        }
        
        public Either<RfcError, IDisposable> AddFunctionHandler(string sysid, 
            IFunctionDescriptionHandle descriptionHandle, Func<IRfcHandle, IFunction, EitherAsync<RfcError, Unit>> handler)
        {
            var holder = Api.RegisterServerFunctionHandler(sysid,
                descriptionHandle as FunctionDescriptionHandle,
                (rfcHandle, functionHandle) =>
                {
                    var func = new Function(functionHandle, this);
                    return handler(rfcHandle, func).MapLeft(l=>l.RfcErrorInfo);
                },
                out var errorInfo);

            return ResultOrError(holder, errorInfo);
        }

        public Either<RfcError, Unit> Invoke(IConnectionHandle connectionHandle, IFunctionHandle functionHandle)
        {
            Logger.IfSome(l => l.LogTrace("Invoking function", new { connectionHandle, functionHandle }));
            var rc = Api.Invoke(connectionHandle as ConnectionHandle, functionHandle as FunctionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, Unit> CancelConnection(IConnectionHandle connectionHandle)
        {
            Logger.IfSome(l => l.LogTrace("cancelling function", new { connectionHandle }));
            var rc = Api.CancelConnection(connectionHandle as ConnectionHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, bool> IsConnectionHandleValid(IConnectionHandle connectionHandle)
        {
            Logger.IfSome(l => l.LogTrace("checking connection state", new { connectionHandle }));
            var rc = Api.IsConnectionHandleValid(connectionHandle as ConnectionHandle, out var isValid, out var errorInfo);
            return ResultOrError(isValid, rc, errorInfo);
        }

        public Either<RfcError, ConnectionAttributes> GetConnectionAttributes(IConnectionHandle connectionHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading connection attributes", new { connectionHandle }));
            var rc = Api.GetConnectionAttributes(connectionHandle as ConnectionHandle, out var attributes, out var errorInfo);
            return ResultOrError(attributes, rc, errorInfo);
        }

        public Either<RfcError, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating structure by data container handle and name", new { dataContainer, name }));
            var rc = Api.GetStructure(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((IStructureHandle)result, rc, errorInfo);

        }

        public Either<RfcError, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle)
        {
            Logger.IfSome(l => l.LogTrace("creating structure by type description handle", new { typeDescriptionHandle }));
            var handle = Api.CreateStructure(typeDescriptionHandle as TypeDescriptionHandle, out var errorInfo);
            return ResultOrError((IStructureHandle) handle, errorInfo.Code, errorInfo);

        }

        public Either<RfcError, Unit> SetStructure(IStructureHandle structureHandle, string content)
        {
            Logger.IfSome(l => l.LogTrace("setting structure by from string", new { content }));
            var rc = Api.SetStructure(structureHandle as StructureHandle, content, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
        {
            Logger.IfSome(l => l.LogTrace("creating table by data container handle and name", new { dataContainer, name }));
            var rc = Api.GetTable(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError((ITableHandle)result, rc, errorInfo);

        }

        public Either<RfcError, ITableHandle> CloneTable(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("cloning table by tableHandle", tableHandle));
            ITableHandle handle = Api.CloneTable(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        [Obsolete("Use method WithStartProgramCallback of ConnectionBuilder. This method will be removed in next major release.")]
        public Either<RfcError, Unit> AllowStartOfPrograms(IConnectionHandle connectionHandle,
            StartProgramDelegate callback)
        {
            Logger.IfSome(l => l.LogTrace("Setting allow start of programs callback"));
            Api.AllowStartOfPrograms(connectionHandle as ConnectionHandle, callback, out var errorInfo);
            return ResultOrError(Unit.Default, errorInfo.Code, errorInfo);

        }

        public Either<RfcError, int> GetTableRowCount(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading table row count by table handle", tableHandle));
            var rc = Api.GetTableRowCount(tableHandle as TableHandle, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("reading current table row by table handle", tableHandle));
            IStructureHandle handle = Api.GetCurrentTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("append table row by table handle", tableHandle));
            IStructureHandle handle = Api.AppendTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(handle, errorInfo);

        }

        public Either<RfcError, Unit> MoveToNextTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to next table row by table handle", tableHandle));
            var rc = Api.MoveToNextTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
        {
            Logger.IfSome(l => l.LogTrace("move to first table row by table handle", tableHandle));
            var rc = Api.MoveToFirstTableRow(tableHandle as TableHandle, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, Unit> SetString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting string value by name", new { containerHandle, name, value}));
            var rc = Api.SetString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, string> GetString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading string value by name", new { containerHandle, name}));
            var rc = Api.GetString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting date string value by name", new { containerHandle, name, value }));
            var rc = Api.SetDateString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, string> GetDateString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("reading date string value by name", new { containerHandle, name }));
            var rc = Api.GetDateString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Either<RfcError, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
            string value)
        {
            Logger.IfSome(l => l.LogTrace("setting time string value by name", new { containerHandle, name, value }));
            var rc = Api.SetTimeString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, string> GetTimeString(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting time string value by name", new { containerHandle, name }));
            var rc = Api.GetTimeString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);

        }

        public Option<ILogger> Logger { get; }

        public Either<RfcError, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func)
        {
            return func().Bind(fieldInfo =>
            {
                Logger.IfSome(l => l.LogTrace("setting field value", new { handle, fieldInfo, SourceType= typeof(T) }));
                return FieldMapper.SetField(value, new FieldMappingContext(this, handle, fieldInfo));
            });
            
        }

        public Either<RfcError, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func)
        {
            return func().Bind(fieldInfo =>
            {
                Logger.IfSome(l => l.LogTrace("reading field value", new { handle, fieldInfo, TargetType = typeof(T) }));
                return FieldMapper.GetField<T>(new FieldMappingContext(this, handle, fieldInfo));
            });


        }

        public Either<RfcError, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value)
        {
            Logger.IfSome(l => l.LogTrace("setting int value by name", new { containerHandle, name, value }));
            var rc = Api.SetInt(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcError, int> GetInt(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting int value by name", new { containerHandle, name }));
            var rc = Api.GetInt(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcError, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
        {
            Logger.IfSome(l => l.LogTrace("setting long value by name", new { containerHandle, name, value }));
            var rc = Api.SetLong(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);
        }

        public Either<RfcError, long> GetLong(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting long value by name", new { containerHandle, name }));
            var rc = Api.GetLong(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }

        public Either<RfcError, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
        {
            Logger.IfSome(l => l.LogTrace("setting byte value by name", new { containerHandle, name }));
            var rc = Api.SetBytes(containerHandle as Internal.IDataContainerHandle, name, buffer, (uint) bufferLength, out var errorInfo);
            return ResultOrError(Unit.Default, rc, errorInfo);

        }

        public Either<RfcError, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
        {
            Logger.IfSome(l => l.LogTrace("getting byte value by name", new { containerHandle, name }));
            var rc = Api.GetBytes(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
            return ResultOrError(result, rc, errorInfo);
        }
    }
}