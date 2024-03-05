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
        private readonly IRfcClientConnectionProvider _connectionProvider;
        private readonly bool _disposeProvider;

        [Obsolete("Use RfcContext(IRfcConnectionProvider connectionProvider) instead.")]
        public RfcContext(Func<EitherAsync<RfcError, IConnection>> connectionBuilder)
        {
            _connectionProvider = new RfcClientConnectionProvider(connectionBuilder);
            _disposeProvider = true;
        }

        public RfcContext(IRfcClientConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        private SAPRfcRuntime GetRuntime() => 
            SAPRfcRuntime.New(_connectionProvider);

        private SAPRfcRuntime GetRuntime(CancellationToken token) =>
            SAPRfcRuntime.New(_connectionProvider, CancellationTokenSource.CreateLinkedTokenSource(token));

        /// <inheritdoc />
        public EitherAsync<RfcError, IConnection> GetConnection()
        {
            return SAPRfc<SAPRfcRuntime>.getConnection().ToEither(GetRuntime());
        }

        /// <inheritdoc />
        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function)
        {
            return InvokeFunction(function, CancellationToken.None);
        }

        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken)
        {
            return SAPRfc<SAPRfcRuntime>.invokeFunction(function).ToEither(GetRuntime(cancellationToken));
        }

        public EitherAsync<RfcError, IRfcContext> Ping()
        {
            return Ping(CancellationToken.None);
        }

        public EitherAsync<RfcError, IRfcContext> Ping(CancellationToken cancellationToken)
        {
            return SAPRfc<SAPRfcRuntime>.ping().ToEither(GetRuntime(cancellationToken))
                .Map(_ => (IRfcContext) this );
        }

        public EitherAsync<RfcError, Unit> Commit()
        {
            return Commit(CancellationToken.None);
        }

        public Task<Either<RfcError, IRfcContext>> PingAsync()
        {
            return PingAsync(CancellationToken.None);
        }

        public Task<Either<RfcError, IRfcContext>> PingAsync(CancellationToken cancellationToken)
        {
            return Ping(cancellationToken).ToEither();
        }

        public Task<Either<RfcError, Unit>> CommitAsync()
        {
            return CommitAsync(CancellationToken.None);
        }


        public EitherAsync<RfcError, IFunction> CreateFunction(string name)
        {
            return CreateFunction(name, CancellationToken.None);
        }

        public EitherAsync<RfcError, IFunction> CreateFunction(string name, CancellationToken cancellationToken) =>
            SAPRfc<SAPRfcRuntime>.createFunction(name).ToEither(GetRuntime(cancellationToken));
        
        public EitherAsync<RfcError, Unit> Commit(CancellationToken cancellationToken) =>
            SAPRfc<SAPRfcRuntime>.commit().ToEither(GetRuntime(cancellationToken));

        public EitherAsync<RfcError, Unit> CommitAndWait()
        {
            return CommitAndWait(CancellationToken.None);
        }

        public EitherAsync<RfcError, Unit> CommitAndWait(CancellationToken cancellationToken) =>
            SAPRfc<SAPRfcRuntime>.commitAndWait().ToEither(GetRuntime(cancellationToken));

        public EitherAsync<RfcError, Unit> Rollback()
        {
            return Rollback(CancellationToken.None);
        }

        public EitherAsync<RfcError, Unit> Rollback(CancellationToken cancellationToken) =>
            SAPRfc<SAPRfcRuntime>.rollback().ToEither(GetRuntime(cancellationToken));

        public Task<Either<RfcError, Unit>> CommitAsync(CancellationToken cancellationToken) => 
            Commit(cancellationToken).ToEither();

        public Task<Either<RfcError, Unit>> CommitAndWaitAsync()
        {
            return CommitAndWaitAsync(CancellationToken.None);
        }

        public Task<Either<RfcError, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken) => 
            CommitAndWait(cancellationToken).ToEither();

        public Task<Either<RfcError, Unit>> RollbackAsync()
        {
            return RollbackAsync(CancellationToken.None);
        }

        public Task<Either<RfcError, Unit>> RollbackAsync(CancellationToken cancellationToken) => 
            Rollback(cancellationToken).ToEither();

        public void Dispose()
        {
            if(_disposeProvider)
                _connectionProvider.Dispose();
        }
    }
}