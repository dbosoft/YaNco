using System;
using System.Collections.Generic;
using System.Threading;
using Dbosoft.YaNco.Traits;
using LanguageExt;
using LanguageExt.Effects.Traits;

namespace Dbosoft.YaNco;

/// <summary>
/// Base class for building a RFC clients or servers.
/// </summary>
/// <typeparam name="TBuilder">The builder type for chaining</typeparam>
/// <typeparam name="RT">Runtime type</typeparam>
public abstract class RfcBuilderBase<TBuilder, RT> where RT : struct,
    HasSAPRfc<RT>, HasCancel<RT>
    where TBuilder: RfcBuilderBase<TBuilder, RT>
{
    protected TBuilder Self { get; set; }

    protected readonly List<(string, Action<IFunctionBuilder<RT>>,
        Func<CalledFunction<RT>, Aff<RT, Unit>>)> FunctionHandlers = new();

    /// <summary>
    /// This method registers a callback of type <see cref="StartProgramDelegate"/> 
    /// to handle backend requests to start local programs.
    /// </summary>
    /// <remarks>
    /// The SAP backend can call function RFC_START_PROGRAM on back destination to request
    /// clients to start local programs. This is used a lot in KPRO applications to start saphttp and sapftp.
    /// </remarks>
    /// <param name="startProgramDelegate">Delegate to callback function implementation.</param>
    /// <returns>current instance for chaining</returns>
    public TBuilder WithStartProgramCallback(StartProgramDelegate startProgramDelegate)
    {
        return WithFunctionHandler("RFC_START_PROGRAM", builder => builder
                .AddChar("COMMAND", RfcDirection.Import, 512),
            cf => cf
                .Input(f => f.GetField<string>("COMMAND"))
                .Process(cmd => startProgramDelegate(cmd))
                .NoReply()
        );
    }

    /// <summary>
    /// This method registers a function handler from a <see cref="IFunctionBuilder{RT}"/>
    /// </summary>
    /// <param name="functionName">Name of function</param>
    /// <param name="configureBuilder">action to configure function builder</param>
    /// <param name="calledFunc">function handler</param>
    /// <returns>current instance for chaining</returns>
    /// <remarks>
    /// The metadata of the function is build in the <see cref="IFunctionBuilder{RT}"/>. This allows to register
    /// any kind of function. 
    /// Function handlers are registered process wide (in the SAP NW RFC Library) and mapped to backend system id. 
    /// Multiple registrations of same function and same backend id will therefore have no effect.
    /// </remarks>
    public TBuilder WithFunctionHandler(string functionName,
        Action<IFunctionBuilder<RT>> configureBuilder,
        Func<CalledFunction<RT>, Aff<RT, Unit>> calledFunc)
    {
        FunctionHandlers.Add((functionName, configureBuilder, calledFunc));
        return Self;
    }


    private Action<RfcRuntimeConfigurer<RT>> _configureRuntime = _ => { };

    protected TBuilder ConfigureRuntimeInternal(Action<RfcRuntimeConfigurer<RT>> configure)
    {
        lock (RfcBuilderSync.SyncObject)
        {
            _configureRuntime = configure;
        }

        return Self;
    }

    protected RT CreateRuntime(CancellationTokenSource cancellationTokenSource, Func<SAPRfcRuntimeEnv<SAPRfcRuntimeSettings>, RT> runtimeFactory)
    {

        lock (RfcBuilderSync.SyncObject)
        {
            var runtimeConfigurer = new RfcRuntimeConfigurer<RT>(runtimeFactory);
            _configureRuntime(runtimeConfigurer);
            return runtimeConfigurer.CreateRuntime(cancellationTokenSource);

        }
    }
}