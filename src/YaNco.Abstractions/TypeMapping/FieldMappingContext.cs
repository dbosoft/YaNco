namespace Dbosoft.YaNco.TypeMapping
{
    public class FieldMappingContext
    {
        public readonly IRfcRuntime RfcRuntime;
        public readonly IDataContainerHandle Handle;
        public readonly RfcFieldInfo FieldInfo;

        public FieldMappingContext(IRfcRuntime rfcRuntime, IDataContainerHandle handle, RfcFieldInfo fieldInfo)
        {
            RfcRuntime = rfcRuntime;
            Handle = handle;
            FieldInfo = fieldInfo;
        }
    }
}
