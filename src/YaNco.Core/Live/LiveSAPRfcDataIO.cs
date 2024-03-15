using Dbosoft.YaNco.Internal;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;
using System;

namespace Dbosoft.YaNco.Live;

public readonly struct LiveSAPRfcDataIO : SAPRfcDataIO
{
    public Option<ILogger> Logger { get; }
    public IFieldMapper FieldMapper { get; }
    public RfcRuntimeOptions Options { get; }

    public LiveSAPRfcDataIO(Option<ILogger> logger, IFieldMapper fieldMapper, RfcRuntimeOptions options)
    {
        Logger = logger;
        FieldMapper = fieldMapper;
        Options = options;
    }

    public Either<RfcError, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        var io = this;
        return func().Bind(fieldInfo =>
        {
            io.Logger.IfSome(l => l.LogTrace("setting field value", new { handle, fieldInfo, SourceType = typeof(T) }));
            return io.FieldMapper.SetField(value, new FieldMappingContext(io, handle, fieldInfo));
        });
    }

    public Either<RfcError, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        var io = this;
        return func().Bind(fieldInfo =>
        {
            io.Logger.IfSome(l => l.LogTrace("reading field value", new { handle, fieldInfo, TargetType = typeof(T) }));
            return io.FieldMapper.GetField<T>(new FieldMappingContext(io, handle, fieldInfo));
        });
    }

    public Either<RfcError, Unit> SetInt(
        IDataContainerHandle containerHandle, string name, int value)
    {
        Logger.IfSome(l => l.LogTrace("setting int value by name", new { containerHandle, name, value }));
        var rc = Api.SetInt(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, int> GetInt(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("getting int value by name", new { containerHandle, name }));
        var rc = Api.GetInt(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value)
    {
        Logger.IfSome(l => l.LogTrace("setting long value by name", new { containerHandle, name, value }));
        var rc = Api.SetLong(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);
    }

    public Either<RfcError, long> GetLong(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("getting long value by name", new { containerHandle, name }));
        var rc = Api.GetLong(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength)
    {
        Logger.IfSome(l => l.LogTrace("setting byte value by name", new { containerHandle, name }));
        var rc = Api.SetBytes(containerHandle as Internal.IDataContainerHandle, name, buffer, (uint)bufferLength, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, byte[]> GetBytes(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("getting byte value by name", new { containerHandle, name }));
        var rc = Api.GetBytes(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);
    }

    public Either<RfcError, Unit> SetString(IDataContainerHandle containerHandle, string name, string value)
    {
        Logger.IfSome(l => l.LogTrace("setting string value by name", new { containerHandle, name, value }));
        var rc = Api.SetString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, string> GetString(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("reading string value by name", new { containerHandle, name }));
        var rc = Api.GetString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, Unit> SetDateString(IDataContainerHandle containerHandle, string name, string value)
    {
        Logger.IfSome(l => l.LogTrace("setting date string value by name", new { containerHandle, name, value }));
        var rc = Api.SetDateString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, string> GetDateString(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("reading date string value by name", new { containerHandle, name }));
        var rc = Api.GetDateString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
        string value)
    {
        Logger.IfSome(l => l.LogTrace("setting time string value by name", new { containerHandle, name, value }));
        var rc = Api.SetTimeString(containerHandle as Internal.IDataContainerHandle, name, value, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, string> GetTimeString(IDataContainerHandle containerHandle, string name)
    {
        Logger.IfSome(l => l.LogTrace("getting time string value by name", new { containerHandle, name }));
        var rc = Api.GetTimeString(containerHandle as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, IStructureHandle> GetStructure(IDataContainerHandle dataContainer, string name)
    {
        Logger.IfSome(l => l.LogTrace("creating structure by data container handle and name", new { dataContainer, name }));
        var rc = Api.GetStructure(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, (IStructureHandle)result, rc, errorInfo);

    }

    public Either<RfcError, IStructureHandle> CreateStructure(ITypeDescriptionHandle typeDescriptionHandle)
    {
        Logger.IfSome(l => l.LogTrace("creating structure by type description handle", new { typeDescriptionHandle }));
        var handle = Api.CreateStructure(typeDescriptionHandle as TypeDescriptionHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, (IStructureHandle)handle, errorInfo.Code, errorInfo);

    }

    public Either<RfcError, Unit> SetStructure(IStructureHandle structureHandle, string content)
    {
        Logger.IfSome(l => l.LogTrace("setting structure by from string", new { content }));
        var rc = Api.SetStructure(structureHandle as StructureHandle, content, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, ITableHandle> GetTable(IDataContainerHandle dataContainer, string name)
    {
        Logger.IfSome(l => l.LogTrace("creating table by data container handle and name", new { dataContainer, name }));
        var rc = Api.GetTable(dataContainer as Internal.IDataContainerHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, (ITableHandle)result, rc, errorInfo);

    }

    public Either<RfcError, ITableHandle> CloneTable(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("cloning table by tableHandle", tableHandle));
        ITableHandle handle = Api.CloneTable(tableHandle as TableHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, handle, errorInfo);

    }


    public Either<RfcError, int> GetTableRowCount(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("reading table row count by table handle", tableHandle));
        var rc = Api.GetTableRowCount(tableHandle as TableHandle, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, IStructureHandle> GetCurrentTableRow(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("reading current table row by table handle", tableHandle));
        IStructureHandle handle = Api.GetCurrentTableRow(tableHandle as TableHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, handle, errorInfo);

    }

    public Either<RfcError, IStructureHandle> AppendTableRow(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("append table row by table handle", tableHandle));
        IStructureHandle handle = Api.AppendTableRow(tableHandle as TableHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, handle, errorInfo);

    }

    public Either<RfcError, Unit> MoveToNextTableRow(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("move to next table row by table handle", tableHandle));
        var rc = Api.MoveToNextTableRow(tableHandle as TableHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, Unit> MoveToFirstTableRow(ITableHandle tableHandle)
    {
        Logger.IfSome(l => l.LogTrace("move to first table row by table handle", tableHandle));
        var rc = Api.MoveToFirstTableRow(tableHandle as TableHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, Unit.Default, rc, errorInfo);

    }

    public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IDataContainerHandle dataContainer)
    {
        Logger.IfSome(l => l.LogTrace("reading type description by container handle", dataContainer));
        ITypeDescriptionHandle handle = Api.GetTypeDescription(dataContainer as Internal.IDataContainerHandle, out var errorInfo);
        return IOResult.ResultOrError(Logger, handle, errorInfo);
                    
    }

    public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription(IConnectionHandle connectionHandle, string typeName)
    {
        Logger.IfSome(l => l.LogTrace("reading type description by type name", typeName));
        ITypeDescriptionHandle handle = Api.GetTypeDescription(connectionHandle as ConnectionHandle, typeName, out var errorInfo);
        return IOResult.ResultOrError(Logger, handle, errorInfo);

    }

    public Either<RfcError, int> GetTypeFieldCount(ITypeDescriptionHandle descriptionHandle)
    {
        Logger.IfSome(l => l.LogTrace("reading field count by type description handle", descriptionHandle));
        var rc = Api.GetTypeFieldCount(descriptionHandle as TypeDescriptionHandle, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        int index)
    {
        Logger.IfSome(l => l.LogTrace("reading field description by type description handle and index", new { descriptionHandle, index }));
        var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, index, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }

    public Either<RfcError, RfcFieldInfo> GetTypeFieldDescription(ITypeDescriptionHandle descriptionHandle,
        string name)
    {
        Logger.IfSome(l => l.LogTrace("reading field description by type description handle and name", new { descriptionHandle, name }));
        var rc = Api.GetTypeFieldDescription(descriptionHandle as TypeDescriptionHandle, name, out var result, out var errorInfo);
        return IOResult.ResultOrError(Logger, result, rc, errorInfo);

    }


    public Either<RfcError, T> GetValue<T>(AbapValue abapValue) => FieldMapper.FromAbapValue<T>(abapValue);

    public Either<RfcError, AbapValue> SetValue<T>(T value, RfcFieldInfo fieldInfo) => FieldMapper.ToAbapValue(value, fieldInfo);

}