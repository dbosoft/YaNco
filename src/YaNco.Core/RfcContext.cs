using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    /// <summary>
    /// Default implementation of <see cref="IRfcContext"/>. The RFCContext can be used to invoke
    /// SAP Remote Function Modules.  
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class RfcContext : IRfcContext
    {
        private readonly Func<EitherAsync<RfcErrorInfo, IConnection>> _connectionBuilder;
        private Option<IConnection> _connection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public RfcContext(Func<EitherAsync<RfcErrorInfo, IConnection>> connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        /// <inheritdoc />
        public EitherAsync<RfcErrorInfo, IConnection> GetConnection()
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

        /// <inheritdoc />
        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function)
        {
            return InvokeFunction(function, CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken)
        {
            return GetConnection()
                .Bind(conn => conn.InvokeFunction(function, cancellationToken));            
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping()
        {
            return Ping(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping(CancellationToken cancellationToken)
        {
            return CreateFunction("RFC_PING", cancellationToken)
                .Bind(f=>InvokeFunction(f, cancellationToken))
                .Map(r => (IRfcContext) this );
        }

        public EitherAsync<RfcErrorInfo, Unit> Commit()
        {
            return Commit(CancellationToken.None);
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync()
        {
            return PingAsync(CancellationToken.None);
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync(CancellationToken cancellationToken)
        {
            return Ping(cancellationToken).ToEither();
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync()
        {
            return CommitAsync(CancellationToken.None);
        }


        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name)
        {
            return CreateFunction(name, CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name, CancellationToken cancellationToken) => 
            GetConnection().Bind(conn => conn.CreateFunction(name));

        public EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken) => 
            GetConnection().Bind(conn => conn.Commit(cancellationToken));

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait()
        {
            return CommitAndWait(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken) => 
            GetConnection().Bind(conn => conn.CommitAndWait(cancellationToken));

        public EitherAsync<RfcErrorInfo, Unit> Rollback()
        {
            return Rollback(CancellationToken.None);
        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken) => 
            GetConnection().Bind(conn => conn.Rollback(cancellationToken));

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync(CancellationToken cancellationToken) => 
            Commit(cancellationToken).ToEither();

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync()
        {
            return CommitAndWaitAsync(CancellationToken.None);
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken) => 
            CommitAndWait(cancellationToken).ToEither();

        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync()
        {
            return RollbackAsync(CancellationToken.None);
        }

        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync(CancellationToken cancellationToken) => 
            Rollback(cancellationToken).ToEither();

        public void Dispose()
        {
            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}