using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public class LongValueConverter<T> : IToAbapValueConverter<T>
    {
        public Try<AbapValue> ConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            return Prelude.Try<AbapValue>(() =>
            {
                if (!IsSupportedRfcType(fieldInfo.Type))
                    throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to long value.");

                return new AbapLongValue(fieldInfo, (long)Convert.ChangeType(value, typeof(long), CultureInfo.InvariantCulture));

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
                case RfcType.INT8:
                    return true;
                default:
                    return false;
            }

        }
    }
}