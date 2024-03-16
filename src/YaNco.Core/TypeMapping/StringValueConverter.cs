using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class StringValueConverter<T> : IToAbapValueConverter<T>
{
    public Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo)
    {
        return Prelude.Try<AbapValue>(() =>
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert string to RfcType {fieldInfo.Type} .");

            string stringValue = null;

            if (value is IConvertible convertible)
            {
                stringValue = convertible.GetTypeCode() switch
                {
                    TypeCode.Boolean => Convert.ToBoolean(value, CultureInfo.InvariantCulture) ? "X" : "",
                    _ => (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture)
                };
            }

            return new AbapStringValue(fieldInfo, stringValue);

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
            RfcType.CHAR => true,
            RfcType.NUM => true,
            RfcType.BCD => true,
            RfcType.FLOAT => true,
            RfcType.DECF16 => true,
            RfcType.DECF34 => true,
            RfcType.STRING => true,
            _ => false
        };
    }
}