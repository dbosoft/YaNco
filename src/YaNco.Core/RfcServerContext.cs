using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal class RfcServerContext : IRfcContext
    {
        private readonly IRfcServer _rfcServer;
        private IRfcContext _currentContext;

        public RfcServerContext(IRfcServer rfcServer)
        {
            _rfcServer = rfcServer;
        }

        private EitherAsync<RfcError, IRfcContext> GetContext()
        {
            if (_currentContext == null)
                _currentContext = new RfcContext(_rfcServer.OpenClientConnection);

            return Prelude.RightAsync<RfcError, IRfcContext>(_currentContext);
        }

        private Task<Either<RfcError, IRfcContext>> GetContextAsync()
        {
            return GetContext().ToEither();
        }

        public void Dispose()
        {
            _currentContext?.Dispose();
            _currentContext = null;
        }

        public EitherAsync<RfcError, IFunction> CreateFunction(string name)
        {
            return GetContext().Bind(c => c.CreateFunction(name));
        }

        public EitherAsync<RfcError, IFunction> CreateFunction(string name, CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.CreateFunction(name, cancellationToken));
        }

        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function)
        {
            return GetContext().Bind(c => c.InvokeFunction(function));
        }

        public EitherAsync<RfcError, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.InvokeFunction(function, cancellationToken));
        }

        public EitherAsync<RfcError, IRfcContext> Ping()
        {
            return GetContext().Bind(c => c.Ping());
        }

        public EitherAsync<RfcError, IRfcContext> Ping(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Ping(cancellationToken));
        }

        public EitherAsync<RfcError, Unit> Commit()
        {
            return GetContext().Bind(c => c.Commit());
        }

        public EitherAsync<RfcError, Unit> Commit(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Commit(cancellationToken));
        }

        public EitherAsync<RfcError, Unit> CommitAndWait()
        {
            return GetContext().Bind(c => c.CommitAndWait());
        }

        public EitherAsync<RfcError, Unit> CommitAndWait(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.CommitAndWait(cancellationToken));
        }

        public EitherAsync<RfcError, Unit> Rollback()
        {
            return GetContext().Bind(c => c.Rollback());
        }

        public EitherAsync<RfcError, Unit> Rollback(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Rollback(cancellationToken));
        }

        public Task<Either<RfcError, IRfcContext>> PingAsync()
        {
            return GetContextAsync().BindAsync(c => c.PingAsync());
        }

        public Task<Either<RfcError, IRfcContext>> PingAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.PingAsync(cancellationToken));
        }

        public Task<Either<RfcError, Unit>> CommitAsync()
        {
            return GetContextAsync().BindAsync(c => c.CommitAsync());
        }

        public Task<Either<RfcError, Unit>> CommitAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.CommitAsync(cancellationToken));
        }

        public Task<Either<RfcError, Unit>> CommitAndWaitAsync()
        {
            return GetContextAsync().BindAsync(c => c.CommitAndWaitAsync());
        }

        public Task<Either<RfcError, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.CommitAndWaitAsync(cancellationToken));
        }

        public Task<Either<RfcError, Unit>> RollbackAsync()
        {
            return GetContextAsync().BindAsync(c => c.RollbackAsync());
        }

        public Task<Either<RfcError, Unit>> RollbackAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.RollbackAsync(cancellationToken));
        }

        public EitherAsync<RfcError, IConnection> GetConnection()
        {
            return GetContext().Bind(c => c.GetConnection());
        }
    }
}