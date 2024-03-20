using System.Threading;

namespace Dbosoft.YaNco;

public class SAPRfcRuntimeEnv<TSettings> 
    where TSettings : SAPRfcRuntimeSettings
{
    public readonly CancellationTokenSource Source;
    public readonly CancellationToken Token;
    public readonly TSettings Settings;

    public SAPRfcRuntimeEnv(CancellationTokenSource source, CancellationToken token, TSettings settings)
    {
        Source = source;
        Token = token;
        Settings = settings;
    }

    public SAPRfcRuntimeEnv(CancellationTokenSource source, TSettings settings) 
        : this(source, source.Token, settings)
    {
    }

    /// <summary>
    ///  converts the settings to base runtime settings
    /// </summary>
    public SAPRfcRuntimeEnv<SAPRfcRuntimeSettings> ToRuntimeSettings() =>
        new(Source, Settings);

}