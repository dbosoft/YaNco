using System;
using System.Threading;
using LanguageExt;

namespace Dbosoft.YaNco;

public readonly struct SAPRfcRuntime 
        
    : HasSAPRfcClient<SAPRfcRuntime>

{
    private readonly SAPRuntimeEnv _env;

    /// <summary>
    /// Constructor
    /// </summary>
    private SAPRfcRuntime(SAPRuntimeEnv env) =>
        _env = env;


    /// <summary>
    /// Configuration environment accessor
    /// </summary>
    public SAPRuntimeEnv Env =>
        _env ?? throw new InvalidOperationException("Runtime Env not set.  Perhaps because of using default(Runtime) or new Runtime() rather than Runtime.New()");

    /// <summary>
    /// Constructor function
    /// </summary>
    public static SAPRfcRuntime New(IRfcClientConnectionProvider connectionProvider) =>
        new(new SAPRuntimeEnv(new CancellationTokenSource(), connectionProvider));


    public static SAPRfcRuntime New(IRfcClientConnectionProvider connectionProvider, CancellationTokenSource cancellationTokenSource) =>
        new(new SAPRuntimeEnv(cancellationTokenSource , connectionProvider));


    /// <summary>
    /// Create a new Runtime with a fresh cancellation token
    /// </summary>
    /// <remarks>Used by localCancel to create new cancellation context for its sub-environment</remarks>
    /// <returns>New runtime</returns>
    public SAPRfcRuntime LocalCancel =>
        new(new SAPRuntimeEnv(new CancellationTokenSource(), _env.RfcConnectionProvider));

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

    public Eff<SAPRfcRuntime, IRfcClientConnectionProvider> RfcClientConnectionEff =>
        Prelude.Eff<SAPRfcRuntime, IRfcClientConnectionProvider>(rt => 
            rt._env.RfcConnectionProvider);

}