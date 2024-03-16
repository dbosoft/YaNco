namespace Dbosoft.YaNco.TypeMapping;

public class AbapValue
{
    public readonly RfcFieldInfo FieldInfo;

    protected AbapValue(RfcFieldInfo fieldInfo)
    {
        FieldInfo = fieldInfo;
    }
}