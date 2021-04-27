using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class LongValueConverter<T> : IToAbapValueConverter<T>
    {
        public AbapValue ConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to long value.");

            return new AbapLongValue(fieldInfo, (long)Convert.ChangeType(value, typeof(long), CultureInfo.InvariantCulture));
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
                case RfcType.INT8:
                    return true;
                default:
                    return false;
            }

        }
    }
}