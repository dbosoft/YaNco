using System;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;

namespace Dbosoft.YaNco
{
    public class RfcRuntimeConfigurer
    {
        private Func<ILogger, IFieldMapper, RfcRuntimeOptions, IRfcRuntime>
            _runtimeFactory = (logger, mapper, options) => new RfcRuntime(logger, mapper, options);

        private ILogger _logger;
        private StartProgramDelegate _startProgramDelegate;

        private RfcRuntimeOptions _options = new RfcRuntimeOptions();
        private Action<RfcMappingConfigurer> _configureMapping = (m) => { };

        public RfcRuntimeConfigurer WithLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }

        public RfcRuntimeConfigurer AllowStartOfPrograms(StartProgramDelegate startProgramDelegate)
        {
            _startProgramDelegate = startProgramDelegate;
            return this;
        }

        public RfcRuntimeConfigurer ConfigureOptions(Action<RfcRuntimeOptions> configure)
        {
            configure(_options);
            return this;
        }

        public RfcRuntimeConfigurer ConfigureMapping(Action<RfcMappingConfigurer> configure)
        {
            _configureMapping = configure;
            return this;
        }

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
            
            var runtime = _runtimeFactory(_logger, mapping,_options);

            if (_startProgramDelegate != null)
                runtime.AllowStartOfPrograms(_startProgramDelegate);

            return runtime;
        }
    }
    
}