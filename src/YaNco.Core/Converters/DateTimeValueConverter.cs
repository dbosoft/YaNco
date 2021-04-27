using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class DateTimeValueConverter<T> : IToAbapValueConverter<T>, IFromAbapValueConverter<T>
    {
        public AbapValue ConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert DateTime to RfcType {fieldInfo.Type} .");

            string stringValue;

            var dateTime = (DateTime) Convert.ChangeType(value, typeof(DateTime), CultureInfo.InvariantCulture);

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
        }

        public bool CanConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                return false;

            try
            {
                ConvertFrom(value, fieldInfo);
                return true;
            }
            catch
            {
                return false;
            }
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

        public T ConvertTo(AbapValue abapValue)
        {
            var stringValue = abapValue as AbapStringValue;

            if (stringValue == null)
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

            return (T) Convert.ChangeType(dateTime, typeof(T));
        }

        public bool CanConvertTo(AbapValue abapValue)
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (abapValue.FieldInfo.Type)
            {
                case RfcType.DATE:
                case RfcType.TIME:
                    return true;
                default:
                    return false;
            }
        }
    }
}