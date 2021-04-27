using System;
using System.Collections.Generic;

namespace Dbosoft.YaNco.Converters
{
    public interface IRfcConverterResolver
    {
        IEnumerable<IToAbapValueConverter<T>> GetToRfcConverters<T>(RfcType rfcType);
        IEnumerable<IFromAbapValueConverter<T>> GetFromRfcConverters<T>(RfcType rfcType, Type abapValueType);
    }
}