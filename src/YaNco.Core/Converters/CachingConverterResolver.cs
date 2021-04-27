using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dbosoft.YaNco.Converters
{
    public class CachingConverterResolver : IRfcConverterResolver
    {
        private readonly IRfcConverterResolver _decoratedResolver;

        public CachingConverterResolver(IRfcConverterResolver decoratedResolver)
        {
            _decoratedResolver = decoratedResolver;
        }

        private readonly IDictionary<string, object> _fromRfcConverters = new ConcurrentDictionary<string, object>();
        private readonly IDictionary<string, object> _toRfcConverters = new ConcurrentDictionary<string, object>();

        public IEnumerable<IToAbapValueConverter<T>> GetToRfcConverters<T>(RfcType rfcType)
        {
            var sourceType = typeof(T);
            var key = $"{rfcType}_{sourceType}";

            if (!_toRfcConverters.ContainsKey(key))
            {
                var converters = _decoratedResolver.GetToRfcConverters<T>(rfcType).ToArray();
                _toRfcConverters.Add(key, converters.Length == 0 ? null : converters);

            }

            var entry = _toRfcConverters[key];

            if (entry != null)
                return (IEnumerable<IToAbapValueConverter<T>>)entry;
            return new IToAbapValueConverter<T>[0];


        }

        public IEnumerable<IFromAbapValueConverter<T>> GetFromRfcConverters<T>(RfcType rfcType, Type abapValueType)
        {
            var targetType = typeof(T);
            var key = $"{rfcType}_{targetType}";

            if (!_fromRfcConverters.ContainsKey(key))
            {
                var converters = _decoratedResolver.GetFromRfcConverters<T>(rfcType, abapValueType).ToArray();
                _fromRfcConverters.Add(key, converters.Length == 0 ? null : converters);
            }

            var entry = _fromRfcConverters[key];

            if (entry != null)
                return (IEnumerable<IFromAbapValueConverter<T>>)entry;
            return new IFromAbapValueConverter<T>[0];
        }
    }
}