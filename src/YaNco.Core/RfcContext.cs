using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{

    public class RfcContext : IRfcContext
    {
        private readonly Func<EitherAsync<RfcErrorInfo, IConnection>> _connectionBuilder;
        private Option<IConnection> _connection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public RfcContext(IDictionary<string, string> connectionParams, ILogger logger = null)
        {
            _connectionBuilder = () => Connection.Create(connectionParams, new RfcRuntime(logger));
        }

        public RfcContext(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        private EitherAsync<RfcErrorInfo, IConnection> GetConnection()
        {

            async Task<Either<RfcErrorInfo, IConnection>> GetConnectionAsync()
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    return await _connection
                        .Bind(c => c.Disposed ? Prelude.None : Prelude.Some(c))
                        .MatchAsync(s => Prelude.Right(s),
                            async () =>
                            {
                                var res = await _connectionBuilder().ToEither();
                                res.Map(connection => _connection = Prelude.Some(connection));
                                return res;
                            }).ConfigureAwait(false);
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            return GetConnectionAsync().ToAsync();
        }


        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken = default)
        {
            return GetConnection()
                .Bind(conn => conn.InvokeFunction(function, cancellationToken));            
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping(CancellationToken cancellationToken = default)
        {
            return CreateFunction("RFC_PING", cancellationToken)
                .Bind(f=>InvokeFunction(f, cancellationToken))
                .Map(r => (IRfcContext) this );
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync(CancellationToken cancellationToken = default)
        {
            return Ping(cancellationToken).ToEither();
        }


        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name, CancellationToken cancellationToken=default) => 
            GetConnection().Bind(conn => conn.CreateFunction(name));

        public EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken = default) => 
            GetConnection().Bind(conn => conn.Commit(cancellationToken));

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken = default) => 
            GetConnection().Bind(conn => conn.CommitAndWait(cancellationToken));

        public EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken = default) => 
            GetConnection().Bind(conn => conn.Rollback(cancellationToken));

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync(CancellationToken cancellationToken = default) => 
            Commit(cancellationToken).ToEither();

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken = default) => 
            CommitAndWait(cancellationToken).ToEither();
        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync(CancellationToken cancellationToken = default) => 
            Rollback(cancellationToken).ToEither();

        public void Dispose()
        {
            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}