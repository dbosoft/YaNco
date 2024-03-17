using System.Text;
using Dbosoft.YaNco;
using Dbosoft.YaNco.Live;
using ExportMATMAS.MaterialMaster;
using LanguageExt;
using LanguageExt.Sys.Traits;

namespace ExportMATMAS;

public readonly struct ConsoleRuntime :
    HasConsole<ConsoleRuntime>,
    HasFile<ConsoleRuntime>,
    HasSAPRfcLogger<ConsoleRuntime>,
    HasSAPRfcData<ConsoleRuntime>,
    HasSAPRfcFunctions<ConsoleRuntime>,
    HasSAPRfcConnection<ConsoleRuntime>,
    HasSAPRfcServer<ConsoleRuntime>,
    HasMaterialManager<ConsoleRuntime>,
    IHasEnvRuntimeSettings

{

    private readonly SAPRfcRuntimeEnv<SAPServerSettings> _env;

    /// <summary>
    /// Constructor
    /// </summary>
    private ConsoleRuntime(SAPRfcRuntimeEnv<SAPServerSettings> env) =>
        _env = env;


    /// <summary>
    /// Configuration environment accessor
    /// </summary>
    public SAPRfcRuntimeEnv<SAPServerSettings> Env =>
        _env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

    SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> IHasEnvRuntimeSettings.Env =>
        Env.ToRuntimeSettings();


    /// <summary>
    /// Constructor function
    /// </summary>
    public static ConsoleRuntime New(CancellationTokenSource cancellationTokenSource, SAPServerSettings settings) =>
        new(new SAPRfcRuntimeEnv<SAPServerSettings>(cancellationTokenSource, settings));


    /// <summary>
    /// Create a new Runtime with a fresh cancellation token
    /// </summary>
    /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
    /// <returns>New runtime</returns>
    public ConsoleRuntime LocalCancel =>
        new(new SAPRfcRuntimeEnv<SAPServerSettings>(new CancellationTokenSource(), Env.Settings));

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

    public Eff<ConsoleRuntime, ConsoleIO> ConsoleEff =>
        Prelude.SuccessEff(LanguageExt.Sys.Live.ConsoleIO.Default);

    public Encoding Encoding => Encoding.UTF8;

    public Eff<ConsoleRuntime, FileIO> FileEff => Prelude.SuccessEff(LanguageExt.Sys.Live.FileIO.Default);

    public Option<ILogger> RfcLogger => Env.Settings.Logger == null ? Option<ILogger>.None : Prelude.Some(Env.Settings.Logger);

    private SAPRfcDataIO DataIO => Env.Settings.RfcDataIO ?? new LiveSAPRfcDataIO(RfcLogger, Env.Settings.FieldMapper, Env.Settings.Options);
    private SAPRfcFunctionIO FunctionIO => Env.Settings.RfcFunctionIO ?? new LiveSAPRfcFunctionIO(RfcLogger, DataIO);
    private SAPRfcConnectionIO ConnectionIO => Env.Settings.RfcConnectionIO ?? new LiveSAPRfcConnectionIO(RfcLogger);
    private SAPRfcServerIO ServerIO => Env.Settings.RfcServerIO ?? new LiveSAPRfcServerIO(RfcLogger);

    public Eff<ConsoleRuntime, Option<ILogger>> RfcLoggerEff => Prelude.Eff<ConsoleRuntime, Option<ILogger>>(rt => rt.RfcLogger);

    public Eff<ConsoleRuntime, SAPRfcDataIO> RfcDataEff => Prelude.Eff<ConsoleRuntime, SAPRfcDataIO>(rt => rt.DataIO);


    public Eff<ConsoleRuntime, SAPRfcFunctionIO> RfcFunctionsEff =>
        Prelude.Eff<ConsoleRuntime, SAPRfcFunctionIO>(rt => rt.FunctionIO);

    public Eff<ConsoleRuntime, SAPRfcConnectionIO> RfcConnectionEff => Prelude.Eff<ConsoleRuntime, SAPRfcConnectionIO>(
        rt => rt.ConnectionIO);

    public Eff<ConsoleRuntime, SAPRfcServerIO> RfcServerEff => Prelude.Eff<ConsoleRuntime, SAPRfcServerIO>(
        rt => rt.ServerIO);

    public Eff<ConsoleRuntime, TransactionManager<MaterialMasterRecord>> MaterialManagerEff =>
        Prelude.Eff<ConsoleRuntime, TransactionManager<MaterialMasterRecord>>(rt =>
            rt.Env.Settings.TaManager);
}