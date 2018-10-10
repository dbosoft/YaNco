using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public class RfcContext : IRfcContext
    {

        private Connection _connection;

        public RfcContext(Connection connection)
        {
            _connection = connection;

        }

        public Task<Either<RfcErrorInfo, Unit>> InvokeFunction(IFunction function)
        {
            return _connection.InvokeFunction(function);            
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> Ping()
        {
            return CreateFunction("RFC_PING")
                .BindAsync(InvokeFunction)
                .MapAsync(r => (IRfcContext) this );
        }

        public Task<Either<RfcErrorInfo, IFunction>> CreateFunction(string name) => _connection.CreateFunction(name);

        public Task<Either<RfcErrorInfo, Unit>> Commit() => _connection.Commit();

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWait() => _connection.CommitAndWait();

        public Task<Either<RfcErrorInfo, Unit>> Rollback() => _connection.Rollback();


        public void Dispose()
        {
            _connection?.Rollback().ConfigureAwait(false).GetAwaiter().GetResult();
            _connection?.Dispose();
            _connection = null;
        }
    }
}