using System;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco
{
    public class RfcRuntimeConfigurer
    {
        private Func<ILogger, IFieldMapper, IRfcRuntime>
            _runtimeFactory = (logger, mapper) => new RfcRuntime(logger, mapper);

        private ILogger _logger;
        private Action<RfcMappingConfigurer> _configureMapping = (m) => { };

        public RfcRuntimeConfigurer WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }


        public RfcRuntimeConfigurer ConfigureMapping(Action<RfcMappingConfigurer> configure)
        {
            _configureMapping = configure;
            return this;
        }

        public RfcRuntimeConfigurer UseFactory(Func<ILogger, IFieldMapper, IRfcRuntime> factory)
        {
            _runtimeFactory = factory;
            return this;
        }

        internal IRfcRuntime Create()
        {
            var mappingConfigurer = new RfcMappingConfigurer();
            _configureMapping(mappingConfigurer);
            var mapping = mappingConfigurer.Create();
            
            return _runtimeFactory(_logger, mapping);
        }
    }
    
}