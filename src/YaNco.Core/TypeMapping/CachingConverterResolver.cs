using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dbosoft.YaNco.TypeMapping
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
            var key = $"{rfcType}_{sourceType.AssemblyQualifiedName}";

            if (!_toRfcConverters.TryGetValue(key, out var entry))
            {
                var converters = _decoratedResolver.GetToRfcConverters<T>(rfcType).ToArray();
                entry = converters.Length == 0 ? null : converters;
                try
                {
                    _toRfcConverters.Add(key, entry);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (entry != null)
                return (IEnumerable<IToAbapValueConverter<T>>)entry;
            return Array.Empty<IToAbapValueConverter<T>>();


        }

        public IEnumerable<IFromAbapValueConverter<T>> GetFromRfcConverters<T>(RfcType rfcType, Type abapValueType)
        {
            var targetType = typeof(T);
            var key = $"{rfcType}_{targetType.AssemblyQualifiedName}";

            if (!_fromRfcConverters.TryGetValue(key, out var entry))
            {
                var converters = _decoratedResolver.GetFromRfcConverters<T>(rfcType, abapValueType).ToArray();
                entry = converters.Length == 0 ? null : converters;
                try
                {
                    _fromRfcConverters.Add(key, entry);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            if (entry != null)
                return (IEnumerable<IFromAbapValueConverter<T>>)entry;
            return Array.Empty<IFromAbapValueConverter<T>>();
        }

    }
}