using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class DefaultFromAbapValueConverter<T> : IFromAbapValueConverter<T>
{
    public Try<T> ConvertTo(AbapValue abapValue)
    {
        return Prelude.Try( () => (T) Convert.ChangeType(abapValue, typeof(T), CultureInfo.InvariantCulture));
    }

    public bool CanConvertTo(RfcType rfcType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return rfcType switch
        {
            RfcType.CHAR => true,
            RfcType.DATE => true,
            RfcType.BCD => true,
            RfcType.TIME => true,
            RfcType.BYTE => true,
            RfcType.NUM => true,
            RfcType.FLOAT => true,
            RfcType.INT => true,
            RfcType.INT2 => true,
            RfcType.INT1 => true,
            RfcType.DECF16 => true,
            RfcType.DECF34 => true,
            RfcType.STRING => true,
            RfcType.INT8 => true,
            _ => false
        };
    }
}