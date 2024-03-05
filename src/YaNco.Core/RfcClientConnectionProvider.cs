using System;
using System.Threading;
using LanguageExt;

namespace Dbosoft.YaNco;

public sealed class RfcClientConnectionProvider : IRfcClientConnectionProvider
{
    private readonly Func<Aff<IConnection>> _connectionBuilder;
    private readonly SemaphoreSlim _semaphore = new(1);
    private Option<IConnection> _connection;

    public RfcClientConnectionProvider(Func<EitherAsync<RfcError, IConnection>> connectionBuilder)
    {
        _connectionBuilder = () => connectionBuilder().ToAff(l=>l);
    }

    public RfcClientConnectionProvider(Func<Aff<IConnection>> connectionBuilder)
    {
        _connectionBuilder = connectionBuilder;
    }

    public Aff<IConnection> GetConnection()
    {
        return Prelude.AffMaybe(async () =>
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                var res = await _connection
                    .Bind(c => c.Disposed ? Prelude.None : Prelude.Some(c))
                    .Match(Prelude.SuccessAff,
                        () => _connectionBuilder()
                    ).Run();
                res.IfSucc(s =>
                {
                    _connection = Prelude.Some(s);
                });

                return res;
            }
            finally
            {
                _semaphore.Release();
            }
        });

    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _connection.IfSome(c => c.Dispose());
    }
}