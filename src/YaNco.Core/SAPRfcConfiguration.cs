using System;
using System.Collections.Generic;
using LanguageExt;

namespace Dbosoft.YaNco;

public class SAPConnection
{
    private readonly IDictionary<string, string> _connectionParam;

    public SAPConnection(IDictionary<string, string> connectionParam)
    {
        _connectionParam = connectionParam;
    }

    /// <summary>
    /// Creates a new <see cref="IRfcClientConnectionProvider"/> to open a client connection
    /// from the given connection parameters and configures the connection using the given builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public IRfcClientConnectionProvider AsRfcClient(Action<ConnectionBuilder> builder)
    {
        var connectionBuilder = new ConnectionBuilder(_connectionParam);
        builder(connectionBuilder);
        return connectionBuilder.GetProvider();
    }

    /// <summary>
    /// Creates a new <see cref="IRfcClientConnectionProvider"/> to open a client connection
    /// from the given connection parameters.
    /// </summary>
    /// <returns></returns>
    public IRfcClientConnectionProvider AsRfcClient()
    {
        var builder = new ConnectionBuilder(_connectionParam);
        return builder.GetProvider();
    }


    /// <summary>
    /// Creates a new <see cref="IRfcClientConnectionProvider"/> to open a client connection
    /// from the given connection parameters and configures the connection using the given builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public EitherAsync<RfcError, IRfcServer> AsRfcServer(Action<ServerBuilder> builder)
    {
        var connectionBuilder = new ServerBuilder(_connectionParam);
        builder(connectionBuilder);
        return connectionBuilder.Build();
    }
}