using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public interface IToAbapValueConverter<T>
    {
        Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo);
        bool CanConvertFrom(RfcType rfcType);

    }
}