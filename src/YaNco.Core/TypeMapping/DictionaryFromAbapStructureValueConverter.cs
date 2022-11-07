using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public class DictionaryFromAbapStructureValueConverter : IFromAbapValueConverter<IDictionary<string, AbapValue>>
    {
        public Try<IDictionary<string, AbapValue>> ConvertTo(AbapValue abapValue)
        {
            return Prelude.Try(() =>
                {
                    if (!(abapValue is AbapStructureValues structure))
                        throw new InvalidCastException(
                            $"cannot convert type of {abapValue.GetType()} to {nameof(AbapStructureValues)}");

                    return structure.Values;
                }

            );
        }

        public bool CanConvertTo(RfcType rfcType)
        {
            
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (rfcType)
            {
                case RfcType.STRUCTURE:
                    return true;
                default:
                    return false;
            }
        }
    }
}