namespace Dbosoft.YaNco.Converters
{
    public interface IToAbapValueConverter<in T>
    {
        AbapValue ConvertFrom(T value, RfcFieldInfo fieldInfo);
        bool CanConvertFrom(T value, RfcFieldInfo fieldInfo);

    }
}