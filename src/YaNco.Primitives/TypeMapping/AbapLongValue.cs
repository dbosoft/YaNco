using System;

namespace Dbosoft.YaNco.TypeMapping;

public class AbapLongValue : AbapValue, IConvertible
{
    public readonly long Value;

    public AbapLongValue(RfcFieldInfo fieldInfo, long value) :
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
        return ((IConvertible) Value).ToBoolean(provider);
    }

    public byte ToByte(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToByte(provider);
    }

    public char ToChar(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToChar(provider);
    }

    public DateTime ToDateTime(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToDateTime(provider);
    }

    public decimal ToDecimal(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToDecimal(provider);
    }

    public double ToDouble(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToDouble(provider);
    }

    public short ToInt16(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToInt16(provider);
    }

    public int ToInt32(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToInt32(provider);
    }

    public long ToInt64(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToInt64(provider);
    }

    public sbyte ToSByte(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToSByte(provider);
    }

    public float ToSingle(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToSingle(provider);
    }

    public string ToString(IFormatProvider provider)
    {
        return Value.ToString(provider);
    }

    public object ToType(Type conversionType, IFormatProvider provider)
    {
        return ((IConvertible) Value).ToType(conversionType, provider);
    }

    public ushort ToUInt16(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToUInt16(provider);
    }

    public uint ToUInt32(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToUInt32(provider);
    }

    public ulong ToUInt64(IFormatProvider provider)
    {
        return ((IConvertible) Value).ToUInt64(provider);
    }
}