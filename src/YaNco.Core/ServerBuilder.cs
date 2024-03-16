using System;
using System.Collections.Generic;
using System.Threading;
using Dbosoft.YaNco.Live;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

/// <summary>
/// This class is used to build server connections to a SAP ABAP backend.
/// <remarks>
/// The non-generic version of this class uses the build-in runtime <see cref="SAPRfcRuntime"/>.
/// To use a custom runtime, use <see cref="ServerBuilder{RT}"/> instead.
/// </remarks>
/// </summary>
[PublicAPI]
public class ServerBuilder : ServerBuilderBase<ServerBuilder, SAPRfcRuntime>
{
    public ServerBuilder(IDictionary<string, string> serverParam) : base(serverParam)
    {
    }

    /// <summary>
    /// Registers a action to configure build-in <see cref="SAPRfcRuntime"/>
    /// </summary>
    /// <param name="configure">action with <see cref="RfcRuntimeConfigurer{SAPRfcRuntime}"/></param>
    /// <returns>current instance for chaining.</returns>
    /// <remarks>
    /// Multiple calls of this method will override the previous configuration action. 
    /// </remarks>
    public ServerBuilder ConfigureRuntime(Action<RfcRuntimeConfigurer<SAPRfcRuntime>> configure)
        => ConfigureRuntimeInternal(configure);

    /// <summary>
    /// This methods builds the <see cref="IRfcServer{SAPRfcRuntime}"/>
    /// with a build-in runtime of type <see cref="SAPRfcRuntime"/>
    /// </summary>
    /// <remarks>
    /// The runtime is created with the configuration actions registered with <see cref="ConfigureRuntime(Action{RfcRuntimeConfigurer{SAPRfcRuntime}})"/>
    /// If you want to use a custom runtime, use <see cref="ServerBuilder{RT}"/> instead.
    /// Multiple calls of this method will return the same server. 
    /// </remarks>
    /// <returns><see cref="EitherAsync{RfcError,A}"/> with the <see cref="IRfcServer{SAPRfcRuntime}"/></returns>

    public new EitherAsync<RfcError, IRfcServer<SAPRfcRuntime>> Build()
    {
        var runtime = CreateRuntime(new CancellationTokenSource(), env => 
            SAPRfcRuntime.New(env.Source, env.Settings));
        return base.Build().ToEither(runtime);
    }

    /// <summary>
    /// This methods builds the async effect to create the <see cref="IRfcServer{SAPRfcRuntime}"/>
    /// </summary>
    /// <remarks>
    /// The runtime is created with the configuration actions registered with <see cref="ConfigureRuntime(Action{RfcRuntimeConfigurer{SAPRfcRuntime}})"/>
    /// If you want to use a custom runtime, use <see cref="ServerBuilder{RT}"/> instead.
    /// Multiple calls of this method will return the same effect. 
    /// </remarks>
    /// <returns><see cref="Aff{SAPRfcRuntime,A}"/> with the <see cref="IRfcServer{SAPRfcRuntime}"/></returns>
    public Aff<SAPRfcRuntime, IRfcServer<SAPRfcRuntime>> BuildIO()
     => base.Build();
}

/// <summary>
/// This method is used to build a server for RFC communication with a SAP ABAP backend.
/// </summary>
/// <typeparam name="RT">The runtime to use</typeparam>
public class ServerBuilder<RT> : ServerBuilderBase<ServerBuilder<RT>, RT>
    where RT : struct, HasSAPRfcServer<RT>,
    HasSAPRfcLogger<RT>, HasSAPRfcData<RT>, HasSAPRfcFunctions<RT>, HasSAPRfcConnection<RT>, HasEnvRuntimeSettings
{
    public ServerBuilder(IDictionary<string, string> serverParam) : base(serverParam)
    {
    }
}