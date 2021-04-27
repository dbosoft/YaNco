using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public interface IToAbapValueConverter<T>
    {
        Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo);
        bool CanConvertFrom(RfcType rfcType);

    }
}