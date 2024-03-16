using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

// ReSharper disable once TypeParameterCanBeVariant
public interface IToAbapValueConverter<T>
{
    Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo);
    bool CanConvertFrom(RfcType rfcType);

}