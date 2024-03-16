using System;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class ByteValueConverter: IToAbapValueConverter<byte[]>, IFromAbapValueConverter<byte[]>
{
    public Try<AbapValue> ConvertFrom(byte[] value, RfcFieldInfo fieldInfo)
    {
        return Prelude.Try<AbapValue>(() =>
        {
            if (!IsSupportedRfcType(fieldInfo.Type))
                throw new NotSupportedException($"Cannot convert from RfcType {fieldInfo.Type} to byte array.");

            return new AbapByteValue(fieldInfo, value);
        });

    }

    public bool CanConvertFrom(RfcType rfcType)
    {
        return IsSupportedRfcType(rfcType);
    }

    public Try<byte[]> ConvertTo(AbapValue abapValue)
    {
        return Prelude.Try(() => (abapValue as AbapByteValue)?.Value);
    }

    public bool CanConvertTo(RfcType rfcType)
    {
        return IsSupportedRfcType(rfcType);
    }

    private static bool IsSupportedRfcType(RfcType rfcType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return rfcType switch
        {
            RfcType.BYTE => true,
            RfcType.XSTRING => true,
            _ => false
        };
    }
}