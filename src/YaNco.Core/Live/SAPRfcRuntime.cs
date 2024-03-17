using System;
using System.Threading;
using Dbosoft.YaNco.Traits;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco.Live;

/// <summary>
/// This is the default runtime that uses the live IO implementations for SAP RFC
/// </summary>
public readonly struct SAPRfcRuntime
        
    : HasSAPRfc<SAPRfcRuntime>,
      HasSAPRfcServer<SAPRfcRuntime>,
      HasCancel<SAPRfcRuntime>

{

    /// <summary>
    /// Static default runtime that can be used to create a runtime with default settings
    /// </summary>
    public static SAPRfcRuntime Default => New(
        new CancellationTokenSource(),
        new SAPRfcRuntimeSettings(null, RfcMappingConfigurer.CreateDefaultFieldMapper(), new RfcRuntimeOptions()));

    private readonly SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> _env;

    /// <summary>
    /// Constructor
    /// </summary>
    private SAPRfcRuntime(SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> env) =>
        _env = env;


    /// <summary>
    /// Configuration environment accessor
    /// </summary>
    public SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> Env =>
        _env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

    /// <summary>
    /// Constructor function
    /// </summary>
    public static SAPRfcRuntime New(CancellationTokenSource cancellationTokenSource, SAPRfcRuntimeSettings settings) =>
            new(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(cancellationTokenSource, settings));    
    
    
    /// <summary>
    /// Creates a new runtime by using the default settings for the environment
    /// </summary>
    public static SAPRfcRuntime New() => new(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(
        new CancellationTokenSource(), new SAPRfcRuntimeSettings(Default.Env.Settings.Logger, 
            Default.Env.Settings.FieldMapper, Default.Env.Settings.Options)
        ));


    /// <summary>
    /// Create a new Runtime with a fresh cancellation token
    /// </summary>
    /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
    /// <returns>New runtime</returns>
    public SAPRfcRuntime LocalCancel =>
        new(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(new CancellationTokenSource(), Env.Settings));

    /// <summary>
    /// Direct access to cancellation token
    /// </summary>
    public CancellationToken CancellationToken =>
        Env.Token;

    /// <summary>
    /// Directly access the cancellation token source
    /// </summary>
    /// <returns>CancellationTokenSource</returns>
    public CancellationTokenSource CancellationTokenSource =>
        Env.Source;

    
    public Option<ILogger> Logger => Env.Settings.Logger == null? Option<ILogger>.None : Prelude.Some(Env.Settings.Logger);

    private SAPRfcDataIO DataIO => Env.Settings.RfcDataIO ?? new LiveSAPRfcDataIO(Logger, Env.Settings.FieldMapper, Env.Settings.Options);
    private SAPRfcFunctionIO FunctionIO => Env.Settings.RfcFunctionIO ?? new LiveSAPRfcFunctionIO(Logger, DataIO);
    private SAPRfcConnectionIO ConnectionIO => Env.Settings.RfcConnectionIO ?? new LiveSAPRfcConnectionIO(Logger);
    private SAPRfcServerIO ServerIO => Env.Settings.RfcServerIO ?? new LiveSAPRfcServerIO(Logger);


    public Eff<SAPRfcRuntime, Option<ILogger>> RfcLoggerEff => Prelude.Eff<SAPRfcRuntime, Option<ILogger>>(rt => rt.Logger);

    public Eff<SAPRfcRuntime, SAPRfcDataIO> RfcDataEff => Prelude.Eff<SAPRfcRuntime, SAPRfcDataIO>(rt => rt.DataIO);


    public Eff<SAPRfcRuntime, SAPRfcFunctionIO> RfcFunctionsEff =>
        Prelude.Eff<SAPRfcRuntime, SAPRfcFunctionIO>(rt => rt.FunctionIO);

    public Eff<SAPRfcRuntime, SAPRfcConnectionIO> RfcConnectionEff => Prelude.Eff<SAPRfcRuntime, SAPRfcConnectionIO>(
        rt =>rt.ConnectionIO);

    public Eff<SAPRfcRuntime, SAPRfcServerIO> RfcServerEff => Prelude.Eff<SAPRfcRuntime, SAPRfcServerIO>(
        rt => rt.ServerIO);

    public Eff<SAPRfcRuntime, IFieldMapper> FieldMapperEff => Prelude.Eff < SAPRfcRuntime, IFieldMapper>(
               rt => rt.Env.Settings.FieldMapper);
}