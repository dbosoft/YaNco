using System;
using System.Collections.Generic;
using System.Threading;
using Dbosoft.YaNco.Live;
using LanguageExt;

namespace Dbosoft.YaNco;

public class ConnectionBuilder : ConnectionBuilderBase<ConnectionBuilder, SAPRfcRuntime>
{
    public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
    {
    }

    public new Func<EitherAsync<RfcError, IConnection>> Build()
    {
        var baseEffect = base.Build();

        return () =>
        {
            var runtime = CreateRuntime(new CancellationTokenSource(), env=> SAPRfcRuntime.New(env.Source, env.Settings));
            return baseEffect.ToEither(runtime);
        };
    }

    public Aff<SAPRfcRuntime, IConnection> BuildIO() => base.Build();

    /// <summary>
    /// Registers a action to configure the <see cref="IRfcRuntime"/>
    /// </summary>
    /// <param name="configure">action with <see cref="RfcRuntimeConfigurer"/></param>
    /// <returns>current instance for chaining.</returns>
    /// <remarks>
    /// Multiple calls of this method will override the previous configuration action. 
    /// </remarks>
    public ConnectionBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer<SAPRfcRuntime>> configure)
        => ConfigureRuntimeInternal(configure);

}


/// <summary>
/// This class is used to build client connections to a SAP ABAP backend.  
/// </summary>
public class ConnectionBuilder<RT> : ConnectionBuilderBase<ConnectionBuilder<RT>, RT>
    where RT : struct, HasSAPRfcFunctions<RT>, HasSAPRfcServer<RT>, HasSAPRfcConnection<RT>, HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasEnvRuntimeSettings

{
    public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
    {
    }
}