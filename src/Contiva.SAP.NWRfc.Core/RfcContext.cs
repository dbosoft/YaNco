using System;
using System.Threading.Tasks;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{

    public class RfcContext : IRfcContext
    {
        private readonly Func<Task<Either<RfcErrorInfo, IConnection>>> _connectionBuilder;
        private Option<IConnection> _connection;

        public RfcContext(Func<Task<Either<RfcErrorInfo, IConnection>>> connectionBuilder)
        {
            _connectionBuilder = connectionBuilder;
        }

        private Task<Either<RfcErrorInfo, IConnection>> GetConnection()
        {
            return _connection.MatchAsync(s => Prelude.Right(s),
                async () =>
                {
                    var res = await _connectionBuilder();
                    res.Map(connection => _connection = Prelude.Some(connection));
                    return res;
                });

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
            if (_connection.IsSome)
                _connection.Map(conn => conn.Rollback().GetAwaiter().GetResult());

            _connection.IfSome(conn => conn.Dispose());
            _connection = Prelude.None;
        }
    }
}