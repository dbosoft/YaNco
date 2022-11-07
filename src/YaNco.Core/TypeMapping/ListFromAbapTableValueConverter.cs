using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public class ListFromAbapTableValueConverter : IFromAbapValueConverter<IEnumerable<IDictionary<string, AbapValue>>>
    {
        public Try<IEnumerable<IDictionary<string, AbapValue>>> ConvertTo(AbapValue abapValue)
        {
            return Prelude.Try(() =>
                {
                    if (!(abapValue is AbapTableValue table))
                        throw new InvalidCastException(
                            $"cannot convert type of {abapValue.GetType()} to {nameof(AbapTableValue)}");

                    return table.Values;
                }

            );
        }

        public bool CanConvertTo(RfcType rfcType)
        {

            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (rfcType)
            {
                case RfcType.TABLE:
                    return true;
                default:
                    return false;
            }
        }
    }
}