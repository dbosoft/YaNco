using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public class DateTimeValueConverter: IToAbapValueConverter<DateTime>, IFromAbapValueConverter<DateTime>
    {
        public Try<AbapValue> ConvertFrom(DateTime value, RfcFieldInfo fieldInfo)
        {
            return Prelude.Try<AbapValue>(() =>
            {
                if (!IsSupportedRfcType(fieldInfo.Type))
                    throw new NotSupportedException($"Cannot convert DateTime to RfcType {fieldInfo.Type} .");

                string stringValue;

                var dateTime = (DateTime)Convert.ChangeType(value, typeof(DateTime), CultureInfo.InvariantCulture);

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (fieldInfo.Type)
                {
                    case RfcType.DATE:
                        stringValue = dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
                        break;
                    case RfcType.TIME:
                        stringValue = dateTime.ToString("HHmmss", CultureInfo.InvariantCulture);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(fieldInfo), fieldInfo.Type,
                            $"not supported Type field in {nameof(fieldInfo)}.");
                }

                return new AbapStringValue(fieldInfo, stringValue);
            });

        }

        public bool CanConvertFrom(RfcType rfcType)
        {
            return IsSupportedRfcType(rfcType);
        }

        private bool IsSupportedRfcType(RfcType rfcType)
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
                if (!(abapValue is AbapStringValue stringValue))
                    throw new ArgumentException($"DateTimeConverter cannot convert type {abapValue.GetType()}",
                        nameof(abapValue));

                DateTime dateTime;
                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (stringValue.FieldInfo.Type)
                {
                    case RfcType.DATE:
                        if (stringValue.Value == "00000000" || stringValue.Value == string.Empty)
                            dateTime = DateTime.MinValue;
                        else
                            dateTime = DateTime.ParseExact(stringValue.Value, "yyyyMMdd", CultureInfo.InvariantCulture);
                        break;
                    case RfcType.TIME:
                        if (stringValue.Value == "000000" || stringValue.Value == string.Empty)
                            dateTime = DateTime.MinValue;
                        else
                            dateTime = default(DateTime).Add(
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
}