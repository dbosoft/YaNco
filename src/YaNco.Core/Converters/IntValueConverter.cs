using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class IntValueConverter<T>: IToAbapValueConverter<T>
    {
        public AbapValue ConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            if(!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to integer value.");

            return new AbapIntValue(fieldInfo, (int)Convert.ChangeType(value, typeof(int), CultureInfo.InvariantCulture));
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
                case RfcType.INT:
                case RfcType.INT2:
                case RfcType.INT1:
                    return true;
                default:
                    return false;
            }

        }
    }
}
