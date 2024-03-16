using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class ListFromAbapTableValueConverter : IFromAbapValueConverter<IEnumerable<IDictionary<string, AbapValue>>>
{
    public Try<IEnumerable<IDictionary<string, AbapValue>>> ConvertTo(AbapValue abapValue)
    {
        return Prelude.Try(() =>
            {
                if (abapValue is not AbapTableValues table)
                    throw new InvalidCastException(
                        $"cannot convert type of {abapValue.GetType()} to {nameof(AbapTableValues)}");

                return table.Values;
            }

        );
    }

    public bool CanConvertTo(RfcType rfcType)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        return rfcType switch
        {
            RfcType.TABLE => true,
            _ => false
        };
    }
}