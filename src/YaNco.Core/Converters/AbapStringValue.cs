using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class AbapStringValue : AbapValue, IConvertible
    {
        public readonly string Value;

        public AbapStringValue(RfcFieldInfo fieldInfo, string value) :
            base(fieldInfo)
        {
            Value = value;
        }

        public TypeCode GetTypeCode()
        {
            return Value.GetTypeCode();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return !string.IsNullOrWhiteSpace(Value);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToByte(CultureInfo.InvariantCulture);
        }

        public char ToChar(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToChar(CultureInfo.InvariantCulture);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return ((IConvertible)Value).ToDateTime(CultureInfo.InvariantCulture);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToDecimal(CultureInfo.InvariantCulture);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToDouble(CultureInfo.InvariantCulture);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToInt16(CultureInfo.InvariantCulture);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToInt32(CultureInfo.InvariantCulture);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToInt64(CultureInfo.InvariantCulture);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToSByte(CultureInfo.InvariantCulture);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToSingle(CultureInfo.InvariantCulture);
        }

        public string ToString(IFormatProvider provider)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return ((IConvertible) Value).ToType(conversionType, CultureInfo.InvariantCulture);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToUInt16(CultureInfo.InvariantCulture);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToUInt32(CultureInfo.InvariantCulture);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((IConvertible) Value).ToUInt64(CultureInfo.InvariantCulture);
        }
    }
}