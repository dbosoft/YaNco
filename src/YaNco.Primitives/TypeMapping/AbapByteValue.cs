namespace Dbosoft.YaNco.TypeMapping
{
    public class AbapByteValue : AbapValue
    {
        public readonly byte[] Value;

        public AbapByteValue(RfcFieldInfo fieldInfo, byte[] value) :
            base(fieldInfo)
        {
            Value = value;
        }
    }
}