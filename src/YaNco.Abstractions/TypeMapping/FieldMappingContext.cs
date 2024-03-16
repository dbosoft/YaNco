namespace Dbosoft.YaNco.TypeMapping;

public class FieldMappingContext
{
    public readonly SAPRfcDataIO IO;
    public readonly IDataContainerHandle Handle;
    public readonly RfcFieldInfo FieldInfo;

    public FieldMappingContext(SAPRfcDataIO io, IDataContainerHandle handle, RfcFieldInfo fieldInfo)
    {
        IO = io;
        Handle = handle;
        FieldInfo = fieldInfo;
    }
}