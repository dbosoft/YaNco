namespace Dbosoft.YaNco;

/// <summary>
/// This interface is used to provide access to the runtime settings of current runtime.
/// It exists for compatibility between OO with build-in runtime and functional runtime.
/// </summary>
/// <remarks>
/// In OO runtime, the runtime settings are provided from defaults that can be overridden by the user.
/// In functional runtime, the runtime settings are provided by the runtime itself.
/// The interface allows to access the runtime settings in a uniform way.
/// </remarks>
public interface IHasEnvRuntimeSettings
{
    SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> Env { get; }
}