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

        private EitherAsync<RfcErrorInfo, IRfcContext> GetContext()
        {
            if (_currentContext == null)
                _currentContext = new RfcContext(_rfcServer.ClientConnection);

            return Prelude.RightAsync<RfcErrorInfo, IRfcContext>(_currentContext);
        }

        private Task<Either<RfcErrorInfo, IRfcContext>> GetContextAsync()
        {
            return GetContext().ToEither();
        }

        public void Dispose()
        {
            _currentContext?.Dispose();
            _currentContext = null;
        }

        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name)
        {
            return GetContext().Bind(c => c.CreateFunction(name));
        }

        public EitherAsync<RfcErrorInfo, IFunction> CreateFunction(string name, CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.CreateFunction(name, cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function)
        {
            return GetContext().Bind(c => c.InvokeFunction(function));
        }

        public EitherAsync<RfcErrorInfo, Unit> InvokeFunction(IFunction function, CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.InvokeFunction(function, cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping()
        {
            return GetContext().Bind(c => c.Ping());
        }

        public EitherAsync<RfcErrorInfo, IRfcContext> Ping(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Ping(cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, Unit> Commit()
        {
            return GetContext().Bind(c => c.Commit());
        }

        public EitherAsync<RfcErrorInfo, Unit> Commit(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Commit(cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait()
        {
            return GetContext().Bind(c => c.CommitAndWait());
        }

        public EitherAsync<RfcErrorInfo, Unit> CommitAndWait(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.CommitAndWait(cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback()
        {
            return GetContext().Bind(c => c.Rollback());
        }

        public EitherAsync<RfcErrorInfo, Unit> Rollback(CancellationToken cancellationToken)
        {
            return GetContext().Bind(c => c.Rollback(cancellationToken));
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync()
        {
            return GetContextAsync().BindAsync(c => c.PingAsync());
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> PingAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.PingAsync(cancellationToken));
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync()
        {
            return GetContextAsync().BindAsync(c => c.CommitAsync());
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.CommitAsync(cancellationToken));
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync()
        {
            return GetContextAsync().BindAsync(c => c.CommitAndWaitAsync());
        }

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWaitAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.CommitAndWaitAsync(cancellationToken));
        }

        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync()
        {
            return GetContextAsync().BindAsync(c => c.RollbackAsync());
        }

        public Task<Either<RfcErrorInfo, Unit>> RollbackAsync(CancellationToken cancellationToken)
        {
            return GetContextAsync().BindAsync(c => c.RollbackAsync(cancellationToken));
        }

        public EitherAsync<RfcErrorInfo, IConnection> GetConnection()
        {
            return GetContext().Bind(c => c.GetConnection());
        }
    }
}