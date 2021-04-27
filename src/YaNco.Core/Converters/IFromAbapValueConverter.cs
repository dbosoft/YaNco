using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public interface IFromAbapValueConverter<T>
    {
        Try<T> ConvertTo(AbapValue abapValue);
        bool CanConvertTo(RfcType rfcType);

    }
}