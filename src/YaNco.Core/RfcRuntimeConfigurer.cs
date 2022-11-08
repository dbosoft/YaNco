using System;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco
{
    public class RfcRuntimeConfigurer
    {
        private Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime>
            _runtimeFactory = (logger, mapper, options) => new RfcRuntime(logger, mapper, options);

        private ILogger _logger;
        private readonly RfcRuntimeOptions _options = new RfcRuntimeOptions();
        private Action<RfcMappingConfigurer> _configureMapping = (m) => { };

        /// <summary>
        /// Registers a <see cref="ILogger"/> implementation as logger in runtime.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Configures options for the created <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer ConfigureOptions(Action<RfcRuntimeOptions> configure)
        {
            configure(_options);
            return this;
        }

        /// <summary>
        /// Configures type mapping of created <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer ConfigureMapping(Action<RfcMappingConfigurer> configure)
        {
            _configureMapping = configure;
            return this;
        }

        /// <summary>
        /// Use a factory method to create the RFC runtime.
        /// </summary>
        /// <param name="factory">function of factory method</param>
        /// <returns></returns>
        /// <remarks>Use this method to override the creation of <see cref="IRfcRuntime"/>. For example for dependency injection.</remarks>
        public RfcRuntimeConfigurer UseFactory(Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime> factory)
        {
            _runtimeFactory = factory;
            return this;
        }

        internal IRfcRuntime Create()
        {
            var mappingConfigurer = new RfcMappingConfigurer();
            _configureMapping(mappingConfigurer);
            var mapping = mappingConfigurer.Create();
            
            return _runtimeFactory(_logger, mapping,_options);
        }
    }
    
}