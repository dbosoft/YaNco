using System;

namespace Dbosoft.YaNco.Converters
{
    public class ByteValueConverter: IToAbapValueConverter<byte[]>, IFromAbapValueConverter<byte[]>
    {
        public AbapValue ConvertFrom(byte[] value, RfcFieldInfo fieldInfo)
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to byte array.");

            return new AbapByteValue(fieldInfo, value);
        }

        public bool CanConvertFrom(byte[] value, RfcFieldInfo fieldInfo)
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
                case RfcType.BYTE:
                case RfcType.XSTRING:
                    return true;
                default:
                    return false;
            }

        }

        public byte[] ConvertTo(AbapValue abapValue)
        {
            return (abapValue as AbapByteValue)?.Value;
        }

        public bool CanConvertTo(AbapValue abapValue)
        {
            return (abapValue is AbapByteValue);
        }
    }
}