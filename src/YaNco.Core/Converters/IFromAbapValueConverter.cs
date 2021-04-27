namespace Dbosoft.YaNco.Converters
{
    public interface IFromAbapValueConverter<out T>
    {
        T ConvertTo(AbapValue abapValue);
        bool CanConvertTo(AbapValue abapValue);

    }
}