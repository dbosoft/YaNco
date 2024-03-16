using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class DateTimeValueConverter: IToAbapValueConverter<DateTime>, IFromAbapValueConverter<DateTime>
{
    public Try<AbapValue> ConvertFrom(DateTime value, RfcFieldInfo fieldInfo)
    {
        return Prelude.Try<AbapValue>(() =>
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert DateTime to RfcType {fieldInfo.Type} .");

            var dateTime = (DateTime)Convert.ChangeType(value, typeof(DateTime), CultureInfo.InvariantCulture);

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            var stringValue = fieldInfo.Type switch
            {
                RfcType.DATE => dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                // ReSharper disable once StringLiteralTypo
                RfcType.TIME => dateTime.ToString("HHmmss", CultureInfo.InvariantCulture),
                _ => throw new ArgumentOutOfRangeException(nameof(fieldInfo), fieldInfo.Type,
                    $"not supported Type field in {nameof(fieldInfo)}.")
            };

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
        switch (rfcType)
        {
            case RfcType.DATE:
            case RfcType.TIME:
                return true;
            default:
                return false;
        }

    }

    public Try<DateTime> ConvertTo(AbapValue abapValue)
    {
        return Prelude.Try(() =>
        {
            if (abapValue is not AbapStringValue stringValue)
                throw new ArgumentException($"DateTimeConverter cannot convert type {abapValue.GetType()}",
                    nameof(abapValue));

            DateTime dateTime;
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (stringValue.FieldInfo.Type)
            {
                case RfcType.DATE:
                    dateTime = stringValue.Value is "00000000" or "" ? DateTime.MinValue : DateTime.ParseExact(stringValue.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                    break;
                case RfcType.TIME:
                    if (stringValue.Value is "000000" or "")
                        dateTime = DateTime.MinValue;
                    else
                        dateTime = default(DateTime).Add(
                            // ReSharper disable once StringLiteralTypo
                            DateTime.ParseExact(stringValue.Value, "HHmmss", CultureInfo.InvariantCulture).TimeOfDay);
                    break;
                default:
                    throw new NotSupportedException(
                        $"It is not supported to convert RfcType {abapValue.FieldInfo.Type} to DateTime");
            }

            return dateTime;

        });

    }

    public bool CanConvertTo(RfcType rfcType)
    {
        return IsSupportedRfcType(rfcType);
    }
}