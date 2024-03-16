using System;
using System.Threading;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco.Test;

public readonly struct TestSAPRfcRuntime
        
    : HasSAPRfcLogger<TestSAPRfcRuntime>,
        HasSAPRfcData<TestSAPRfcRuntime>,
        HasSAPRfcFunctions<TestSAPRfcRuntime>,
        HasSAPRfcConnection<TestSAPRfcRuntime>,
        HasSAPRfcServer<TestSAPRfcRuntime>,
        HasFieldMapper<TestSAPRfcRuntime>,
        HasEnvRuntimeSettings
{
    private readonly SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> _env;
    /// <summary>
    /// Constructor
    /// </summary>
    private TestSAPRfcRuntime(SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> env) =>
        _env = env;


    /// <summary>
    /// Configuration environment accessor
    /// </summary>
    public SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> Env =>
        _env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

    /// <summary>
    /// Constructor function
    /// </summary>
    public static TestSAPRfcRuntime New(SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> env) => new(env);
    public static TestSAPRfcRuntime New(Action<SAPRfcRuntimeSettings> configure)
    {
        var settings = new SAPRfcRuntimeSettings(null, null, new RfcRuntimeOptions());
        configure(settings);
        return New(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(new CancellationTokenSource(),settings));
    }


    /// <summary>
    /// Create a new Runtime with a fresh cancellation token
    /// </summary>
    /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
    /// <returns>New runtime</returns>
    public TestSAPRfcRuntime LocalCancel =>
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

    private SAPRfcDataIO DataIO => Env.Settings.RfcDataIO;
    private SAPRfcFunctionIO FunctionIO => Env.Settings.RfcFunctionIO;
    private SAPRfcConnectionIO ConnectionIO => Env.Settings.RfcConnectionIO;
    private SAPRfcServerIO ServerIO => Env.Settings.RfcServerIO;


    public Eff<TestSAPRfcRuntime, Option<ILogger>> RfcLoggerEff => Prelude.Eff<TestSAPRfcRuntime, Option<ILogger>>(rt => rt.Logger);

    public Eff<TestSAPRfcRuntime, SAPRfcDataIO> RfcDataEff => Prelude.Eff<TestSAPRfcRuntime, SAPRfcDataIO>(rt => rt.DataIO);


    public Eff<TestSAPRfcRuntime, SAPRfcFunctionIO> RfcFunctionsEff =>
        Prelude.Eff<TestSAPRfcRuntime, SAPRfcFunctionIO>(rt => rt.FunctionIO);

    public Eff<TestSAPRfcRuntime, SAPRfcConnectionIO> RfcConnectionEff => Prelude.Eff<TestSAPRfcRuntime, SAPRfcConnectionIO>(
        rt =>rt.ConnectionIO);

    public Eff<TestSAPRfcRuntime, SAPRfcServerIO> RfcServerEff => Prelude.Eff<TestSAPRfcRuntime, SAPRfcServerIO>(
        rt => rt.ServerIO);

    public Eff<TestSAPRfcRuntime, IFieldMapper> FieldMapperEff => Prelude.Eff<TestSAPRfcRuntime, IFieldMapper>(
               rt => rt.Env.Settings.FieldMapper);
}