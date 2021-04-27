namespace Dbosoft.YaNco.Converters
{
    public class AbapValue
    {
        public readonly RfcFieldInfo FieldInfo;

        protected AbapValue(RfcFieldInfo fieldInfo)
        {
            FieldInfo = fieldInfo;
        }
    }
}