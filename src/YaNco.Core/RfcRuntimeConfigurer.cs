using System;
using System.Threading;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco
{
    public class RfcRuntimeConfigurer<RT, TSettings>
        where TSettings : SAPRfcRuntimeSettings
        where RT : struct, HasEnvSettings<TSettings>
    {
        private Func<ILogger, IFieldMapper, RfcTableOptions, TSettings>
            _settingsFactory = (logger, mapper, o) =>
                new SAPRfcRuntimeSettings(logger, mapper,o) as TSettings;

        private Func<SAPRfcRuntimeEnv<TSettings>, RT> _runtimeFactory;

        public RfcRuntimeConfigurer(Func<SAPRfcRuntimeEnv<TSettings>, RT> runtimeFactory)
        {
            _runtimeFactory = runtimeFactory;
        }

        private ILogger _logger;
        private readonly RfcTableOptions _options = new();
        private Action<RfcMappingConfigurer> _configureMapping = (m) => { };

        /// <summary>
        /// Registers a <see cref="ILogger"/> implementation as logger in runtime.
        /// </summary>
        /// <param name="logger"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer<RT,TSettings> WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        /// <summary>
        /// Configures options for the created <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer<RT,TSettings> ConfigureOptions(Action<RfcTableOptions> configure)
        {
            configure(_options);
            return this;
        }

        /// <summary>
        /// Configures type mapping of created <see cref="IRfcRuntime"/>
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public RfcRuntimeConfigurer<RT,TSettings> ConfigureMapping(Action<RfcMappingConfigurer> configure)
        {
            _configureMapping = configure;
            return this;
        }

        /// <summary>
        /// Use a factory method to create the RFC runtime.
        /// </summary>
        /// <param name="factory">function of factory method</param>
        /// <returns></returns>
        /// <remarks>Use this method to override the creation of <see cref="SAPRfcRuntimeSettings"/>. For example for dependency injection.</remarks>
        public RfcRuntimeConfigurer<RT,TSettings> UseSettingsFactory(Func<ILogger, IFieldMapper, RfcTableOptions, TSettings> factory)
        {
            _settingsFactory = factory;
            return this;
        }

        /// <summary>
        /// Use a factory method to create the RFC runtime.
        /// </summary>
        /// <param name="factory">function of factory method</param>
        /// <returns></returns>
        /// <remarks>Use this method to override the creation of <see cref="SAPRfcRuntimeSettings"/>. For example for dependency injection.</remarks>
        public RfcRuntimeConfigurer<RT,TSettings> UseRuntimeFactory(Func<SAPRfcRuntimeEnv<TSettings>,RT> factory)
        {
            _runtimeFactory = factory;
            return this;
        }

        public RT CreateRuntime(CancellationTokenSource cancellationTokenSource)
        {
            var mappingConfigurer = new RfcMappingConfigurer();
            _configureMapping(mappingConfigurer);
            var mapping = mappingConfigurer.Create();
            
            var settings = _settingsFactory(_logger, mapping,_options);
            return _runtimeFactory(new SAPRfcRuntimeEnv<TSettings>(cancellationTokenSource, settings));
        }
    }
    
}