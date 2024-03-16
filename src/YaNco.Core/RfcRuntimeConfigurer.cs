using System;
using System.Threading;
using Dbosoft.YaNco.TypeMapping;
using JetBrains.Annotations;

namespace Dbosoft.YaNco;

[PublicAPI]
public class RfcRuntimeConfigurer<RT>
    where RT : struct
{
    private Func<ILogger, IFieldMapper, RfcRuntimeOptions, SAPRfcRuntimeSettings>
        _settingsFactory = (logger, mapper, o) =>
            new SAPRfcRuntimeSettings(logger, mapper,o);

    private Func<SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>, RT> _runtimeFactory;

    public RfcRuntimeConfigurer(Func<SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>, RT> runtimeFactory)
    {
        _runtimeFactory = runtimeFactory;
    }

    private ILogger _logger;
    private readonly RfcRuntimeOptions _options = new();
    private Action<RfcMappingConfigurer> _configureMapping = _ => { };

    /// <summary>
    /// Registers a <see cref="ILogger"/> implementation as logger in runtime.
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    public RfcRuntimeConfigurer<RT> WithLogger(ILogger logger)
    {
        _logger = logger;
        return this;
    }

    /// <summary>
    /// Configures options for the created <see cref="IRfcRuntime"/>
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public RfcRuntimeConfigurer<RT> ConfigureOptions(Action<RfcRuntimeOptions> configure)
    {
        configure(_options);
        return this;
    }

    /// <summary>
    /// Configures type mapping of created <see cref="IRfcRuntime"/>
    /// </summary>
    /// <param name="configure"></param>
    /// <returns></returns>
    public RfcRuntimeConfigurer<RT> ConfigureMapping(Action<RfcMappingConfigurer> configure)
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
    public RfcRuntimeConfigurer<RT> UseSettingsFactory(Func<ILogger, IFieldMapper, RfcRuntimeOptions, SAPRfcRuntimeSettings> factory)
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
    public RfcRuntimeConfigurer<RT> UseRuntimeFactory(Func<SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>,RT> factory)
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
        return _runtimeFactory(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(cancellationTokenSource, settings));
    }
}