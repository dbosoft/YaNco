using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public interface IFromAbapValueConverter<T>
    {
        Try<T> ConvertTo(AbapValue abapValue);
        bool CanConvertTo(RfcType rfcType);

    }
}