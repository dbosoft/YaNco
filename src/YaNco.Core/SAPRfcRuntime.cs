using System;
using System.Threading;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco;

public readonly struct SAPRfcRuntime
        
    : HasSAPRfcLogger<SAPRfcRuntime>,
        HasSAPRfcData<SAPRfcRuntime>,
        HasSAPRfcFunctions<SAPRfcRuntime>,
        HasSAPRfcConnection<SAPRfcRuntime>,
        HasSAPRfcServer<SAPRfcRuntime>,
        HasCancelFactory<SAPRfcRuntime>,
        HasEnvSettings<SAPRfcRuntimeSettings>

{
    public static SAPRfcRuntime Default => new();

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
    public static SAPRfcRuntime New(SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> env) => new(env);


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

    private SAPRfcDataIO DataIO => new LiveSAPRfcDataIO(Logger, Env.Settings.FieldMapper, Env.Settings.TableOptions);
    private SAPRfcFunctionIO FunctionIO => new LiveSAPRfcFunctionIO(Logger, DataIO);
    private SAPRfcConnectionIO ConnectionIO => new LiveSAPRfcConnectionIO(Logger);
    private SAPRfcServerIO ServerIO => new LiveSAPRfcServerIO();


    public Eff<SAPRfcRuntime, Option<ILogger>> RfcLoggerEff => Prelude.Eff<SAPRfcRuntime, Option<ILogger>>(rt => rt.Logger);

    public Eff<SAPRfcRuntime, SAPRfcDataIO> RfcDataEff => Prelude.Eff<SAPRfcRuntime, SAPRfcDataIO>(rt => rt.DataIO);


    public Eff<SAPRfcRuntime, SAPRfcFunctionIO> RfcFunctionsEff =>
        Prelude.Eff<SAPRfcRuntime, SAPRfcFunctionIO>(rt => rt.FunctionIO);

    public Eff<SAPRfcRuntime, SAPRfcConnectionIO> RfcConnectionEff => Prelude.Eff<SAPRfcRuntime, SAPRfcConnectionIO>(
        rt =>rt.ConnectionIO);

    public Eff<SAPRfcRuntime, SAPRfcServerIO> RfcServerEff => Prelude.Eff<SAPRfcRuntime, SAPRfcServerIO>(
        rt => rt.ServerIO);

    public SAPRfcRuntime WithCancelToken(CancellationToken token)
    {
        return New(new SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>(Env.Source, token,
             Env.Settings));
    }
}