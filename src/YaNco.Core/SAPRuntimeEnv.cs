using System;
using System.Threading;

namespace Dbosoft.YaNco;

public class SAPRuntimeEnv : IDisposable    
{
    public readonly CancellationTokenSource Source;
    public readonly CancellationToken Token;
    public readonly IRfcClientConnectionProvider RfcConnectionProvider;

    public SAPRuntimeEnv(CancellationTokenSource source, CancellationToken token,
        IRfcClientConnectionProvider rfcConnectionProvider)
    {
        Source = source;
        Token = token;
        RfcConnectionProvider = rfcConnectionProvider;
    }

    public SAPRuntimeEnv(CancellationTokenSource source, IRfcClientConnectionProvider rfcConnectionProvider) 
        : this(source, source.Token, rfcConnectionProvider)
    {
    }

    public void Dispose()
    {
        Source?.Dispose();
    }
}