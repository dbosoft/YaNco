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
        switch (rfcType)
        {
            case RfcType.CHAR:
            case RfcType.DATE:
            case RfcType.BCD:
            case RfcType.TIME:
            case RfcType.BYTE:
            case RfcType.NUM:
            case RfcType.FLOAT:
            case RfcType.INT:
            case RfcType.INT2:
            case RfcType.INT1:
            case RfcType.DECF16:
            case RfcType.DECF34:
            case RfcType.STRING:
            case RfcType.INT8:
                return true;
            default:
                return false;
        }
    }
}