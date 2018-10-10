using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{

    public class RfcContext : IRfcContext
    {
        private readonly Func<Task<IConnection>> _connectionBuilder;
        private Option<IConnection> _connection;

        public RfcContext(Func<Task<IConnection>> connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        private Task<IConnection> GetConnection()
        {
            return _connection.MatchAsync(s => s,
                async () =>
                {
                    var res = await _connectionBuilder();
                    _connection = Prelude.Some(res);
                    return res;
                });

        }


public Task<Either<RfcErrorInfo, Unit>> InvokeFunction(IFunction function)
        {
            return GetConnection().MapAsync(conn => conn.InvokeFunction(function));            
        }

        public Task<Either<RfcErrorInfo, IRfcContext>> Ping()
        {
            return CreateFunction("RFC_PING")
                .BindAsync(InvokeFunction)
                .MapAsync(r => (IRfcContext) this );
        }

        public Task<Either<RfcErrorInfo, IFunction>> CreateFunction(string name) => GetConnection().MapAsync(conn => conn.CreateFunction(name));

        public Task<Either<RfcErrorInfo, Unit>> Commit() => GetConnection().MapAsync(conn => conn.Commit());

        public Task<Either<RfcErrorInfo, Unit>> CommitAndWait() => GetConnection().MapAsync(conn => conn.CommitAndWait());

        public Task<Either<RfcErrorInfo, Unit>> Rollback() => GetConnection().MapAsync(conn => conn.Rollback());


        public void Dispose()
        {
            if (_connection.IsSome)
                _connection.Map(conn => conn.Rollback().GetAwaiter().GetResult());

            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}