using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;

namespace Dbosoft.SAP.NWRfc
{

    public class RfcContext : IRfcContext
    {
        private readonly Func<Task<Either<RfcErrorInfo, IConnection>>> _connectionBuilder;
        private Option<IConnection> _connection;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public RfcContext(Func<Task<Either<RfcErrorInfo, IConnection>>> connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        private async Task<Either<RfcErrorInfo, IConnection>> GetConnection()
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                return await _connection.MatchAsync(s => Prelude.Right(s),
                    async () =>
                    {
                        var res = await _connectionBuilder();
                        res.Map(connection => _connection = Prelude.Some(connection));
                        return res;
                    }).ConfigureAwait(false);
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public Task<Either<RfcErrorInfo, Unit>> InvokeFunction(IFunction function)
        {
            return GetConnection().BindAsync(conn => conn.InvokeFunction(function));            
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> Ping()
        {
            return CreateFunction("RFC_PING")
                .BindAsync(InvokeFunction)
                .MapAsync(r => (IRfcContext) this );
        }

        public Task<Either<RfcErrorInfo, IFunction>> CreateFunction(string name) => GetConnection().BindAsync(conn => conn.CreateFunction(name));

        public Task<Either<RfcErrorInfo, Unit>> Commit() => GetConnection().BindAsync(conn => conn.Commit());

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWait() => GetConnection().BindAsync(conn => conn.CommitAndWait());

        public Task<Either<RfcErrorInfo, Unit>> Rollback() => GetConnection().BindAsync(conn => conn.Rollback());


        public void Dispose()
        {
            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}