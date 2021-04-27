using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class StringValueConverter<T> : IToAbapValueConverter<T>
    {
        public AbapValue ConvertFrom(T value, RfcFieldInfo fieldInfo)
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert string to RfcType {fieldInfo.Type} .");

            string stringValue = null;

            if (value is IConvertible convertible)
            {
                switch (convertible.GetTypeCode())
                {
                    case TypeCode.Boolean:
                        stringValue = Convert.ToBoolean(value, CultureInfo.InvariantCulture) ? "X" : "";
                        break;
                    default:
                        stringValue = (string) Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
                        break;
                }
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
                case RfcType.CHAR:
                case RfcType.NUM:
                case RfcType.BCD:
                case RfcType.FLOAT:
                case RfcType.DECF16:
                case RfcType.DECF34:
                case RfcType.STRING:
                    return true;
                default:
                    return false;
            }

        }
    }
}