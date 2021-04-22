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
                    return await _connection.MatchAsync(s => Prelude.Right(s),
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


        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function)
        {
            return GetConnection()
                .Bind(conn => conn.InvokeFunction(function));            
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping()
        {
            return CreateFunction("RFC_PING")
                .Bind(InvokeFunction)
                .Map(r => (IRfcContext) this );
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync()
        {
            return Ping().ToEither();
        }


        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name) => GetConnection().Bind(conn => conn.CreateFunction(name));

        public EitherAsync<RfcErrorInfo, Unit> Commit() => GetConnection().Bind(conn => conn.Commit());

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait() => GetConnection().Bind(conn => conn.CommitAndWait());

        public EitherAsync<RfcErrorInfo, Unit> Rollback() => GetConnection().Bind(conn => conn.Rollback());

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync() => Commit().ToEither();

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync() => CommitAndWait().ToEither();
        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync() => Rollback().ToEither();

        public void Dispose()
        {
            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}