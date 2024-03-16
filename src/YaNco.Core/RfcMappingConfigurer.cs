using System;
using System.Collections.Generic;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco
{
    public class RfcMappingConfigurer
    {
        private Func<IEnumerable<Type>, IEnumerable<Type>,IFieldMapper>
            _mappingFactory = CreateDefaultFieldMapper;

        private readonly List<Type> _fromRfcMappingTypes = new List<Type>();
        private readonly List<Type> _toRfcMappingTypes = new List<Type>();


        public static IFieldMapper CreateDefaultFieldMapper(IEnumerable<Type> fromRfcConverters = null,
            IEnumerable<Type> toRfcConverters = null)
        {
            return new DefaultFieldMapper(
                new CachingConverterResolver(
                    DefaultConverterResolver.CreateWithBuildInConverters(fromRfcConverters, toRfcConverters)));
        }

        public RfcMappingConfigurer UseFactory(Func<IEnumerable<Type>, IEnumerable<Type>, IFieldMapper> factory)
        {
            _mappingFactory = factory;
            return this;
        }

        public RfcMappingConfigurer AddToRfcMapper(Type mapper)
        {
            _toRfcMappingTypes.Add(mapper);
            return this;
        }

        public RfcMappingConfigurer AddFromRfcMapper(Type mapper)
        {
            _fromRfcMappingTypes.Add(mapper);
            return this;
        }

        internal IFieldMapper Create()
        {
            return _mappingFactory(_fromRfcMappingTypes, _toRfcMappingTypes);
        }
    }
}