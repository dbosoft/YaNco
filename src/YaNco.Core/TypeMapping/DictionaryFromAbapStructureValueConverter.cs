using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class DictionaryFromAbapStructureValueConverter : IFromAbapValueConverter<IDictionary<string, AbapValue>>
{
    public Try<IDictionary<string, AbapValue>> ConvertTo(AbapValue abapValue)
    {
        return Prelude.Try(() =>
            {
                if (abapValue is not AbapStructureValues structure)
                    throw new InvalidCastException(
                        $"cannot convert type of {abapValue.GetType()} to {nameof(AbapStructureValues)}");

                return structure.Values;
            }

        );
    }

    public bool CanConvertTo(RfcType rfcType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return rfcType switch
        {
            RfcType.STRUCTURE => true,
            _ => false
        };
    }
}