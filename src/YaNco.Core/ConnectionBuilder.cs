using System;
using System.Collections.Generic;
using System.Threading;
using Dbosoft.YaNco.Live;
using Dbosoft.YaNco.Traits;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

/// <summary>
/// This class is used to build client connections to a SAP ABAP backend.
/// <remarks>
/// The non-generic version of this class uses the build-in runtime <see cref="SAPRfcRuntime"/>.
/// To use a custom runtime, use <see cref="ConnectionBuilder{RT}"/> instead.
/// </remarks>
/// </summary>
public class ConnectionBuilder : ConnectionBuilderBase<ConnectionBuilder, SAPRfcRuntime>
{
    public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
    {
    }

    /// <summary>
    /// This methods builds the <see cref="IConnection"/> factory function
    /// with a build-in runtime of type <see cref="SAPRfcRuntime"/>
    /// </summary>
    /// <remarks>
    /// The runtime is created with the configuration actions registered with <see cref="ConfigureRuntime(Action{RfcRuntimeConfigurer{SAPRfcRuntime}})"/>
    /// If you want to use a custom runtime, use <see cref="ConnectionBuilder{RT}"/> instead.
    /// Multiple calls of this method will return the same factory function.
    /// </remarks>
    /// <returns><see cref="EitherAsync{RfcError,A}"/> with the <see cref="IConnection"/></returns>
    public new Func<EitherAsync<RfcError, IConnection>> Build()
    {
        var baseEffect = base.Build();

        return () =>
        {
            var runtime = CreateRuntime(new CancellationTokenSource(), env=> SAPRfcRuntime.New(env.Source, env.Settings));
            return baseEffect.ToEither(runtime);
        };
    }

    /// <summary>
    /// This methods builds the async effect to create the <see cref="IConnection"/>
    /// </summary>
    /// <remarks>
    /// The runtime is created with the configuration actions registered with <see cref="ConfigureRuntime(Action{RfcRuntimeConfigurer{SAPRfcRuntime}})"/>
    /// If you want to use a custom runtime, use <see cref="IConnection"/> instead.
    /// Multiple calls of this method will return the same effect. 
    /// </remarks>
    /// <returns><see cref="Aff{SAPRfcRuntime,A}"/> with the <see cref="IConnection"/></returns>
    public Aff<SAPRfcRuntime, IConnection> BuildIO() => base.Build();

    /// <summary>
    /// Registers a action to configure the <see cref="IRfcRuntime"/>
    /// </summary>
    /// <param name="configure">action with <see cref="RfcRuntimeConfigurer{SAPRfcRuntime}"/></param>
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
/// <typeparam name="RT">The runtime to be used for the connection</typeparam>
public class ConnectionBuilder<RT> : ConnectionBuilderBase<ConnectionBuilder<RT>, RT>
    where RT : struct, HasSAPRfcServer<RT>, HasSAPRfc<RT>, HasCancel<RT>

{
    public ConnectionBuilder(IDictionary<string, string> connectionParam) : base(connectionParam)
    {
    }
}