using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class IntValueConverter<T>: IToAbapValueConverter<T>
{
    public Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo)
    {
        return Prelude.Try<AbapValue>(() =>
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to integer value.");

            return new AbapIntValue(fieldInfo, (int)Convert.ChangeType(value, typeof(int), CultureInfo.InvariantCulture));

        });

    }

    public bool CanConvertFrom(RfcType rfcType)
    {
        return IsSupportedRfcType(rfcType);
    }

    private static bool IsSupportedRfcType(RfcType rfcType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return rfcType switch
        {
            RfcType.INT => true,
            RfcType.INT2 => true,
            RfcType.INT1 => true,
            _ => false
        };
    }
}