using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Dbosoft.YaNco.Test;
using LanguageExt;
using Moq;
using Xunit;
using YaNco.Core.Tests.RfcMock;
using static Dbosoft.YaNco.SAPRfc<Dbosoft.YaNco.Test.TestSAPRfcRuntime>;

namespace YaNco.Core.Tests
{

    [ExcludeFromCodeCoverage]

    public class ConnectionTests
    {
        [Fact]
        public async Task Connection_is_Opened_And_Disposed()
        {
            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out _);

            var runtime = TestSAPRfcRuntime.New(settings =>
            {
                settings.RfcConnectionIO = connectionIO.Object;
                settings.RfcFunctionIO = new Mock<SAPRfcFunctionIO>().Object;
            });

            var call = from conn in useConnection(NewConnection()
                    , c=> Prelude.SuccessAff(c).WithRuntime<TestSAPRfcRuntime>())
                select conn;
            var finConn = await call.Run(runtime);
            connectionIO.VerifyAll();

            finConn.IfSucc(c => Assert.True(c.Disposed));

        }

        private static Aff<TestSAPRfcRuntime, IConnection> NewConnection() =>
            new ConnectionBuilder<TestSAPRfcRuntime>(new Dictionary<string, string>())
                .Build();

        [Fact]
        public async Task Function_is_created()
        {
            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out var connHandle);
            var functionIO = new Mock<SAPRfcFunctionIO>();
            functionIO.Setup(x => x.GetFunctionDescription(connHandle.Object, "RFC_PING"))
                .Returns(Prelude.Right(new Mock<IFunctionDescriptionHandle>().Object));

            var dataIO = new Mock<SAPRfcDataIO>();


            var runtime = TestSAPRfcRuntime.New(settings =>
            {
                settings.RfcConnectionIO = connectionIO.Object;
                settings.RfcFunctionIO = functionIO.Object;
                settings.RfcDataIO = dataIO.Object;
            });

            var call = from conn in new ConnectionBuilder<TestSAPRfcRuntime>(new Dictionary<string, string>())
                    .Build()
                      from func in createFunction(conn, "RFC_PING" )
                select func;

            var fin = await call.Run(runtime);
            connectionIO.VerifyAll();
            functionIO.VerifyAll();


        }

        [Fact]
        public async Task Commit_is_called()
        {

            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out var connHandle);
            var functionIO = new Mock<SAPRfcFunctionIO>();
            var dataIO = new Mock<SAPRfcDataIO>();

            functionIO.SetupFunction("BAPI_TRANSACTION_COMMIT", connHandle, (h) =>
            {
                var structureHandle = new Mock<IStructureHandle>();

                dataIO.Setup(x => x.GetStructure(h, "RETURN"))
                    .Returns(Prelude.Right(structureHandle.Object));
                dataIO.Setup(x =>
                        x.GetFieldValue<string>(structureHandle.Object,
                            It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                    .Returns(Prelude.Right(""));
            });

            var runtime = TestSAPRfcRuntime.New(settings =>
            {
                settings.RfcConnectionIO = connectionIO.Object;
                settings.RfcFunctionIO = functionIO.Object;
                settings.RfcDataIO = dataIO.Object;
            });

            var call = from conn in useConnection(NewConnection()
                    , commit )
                select conn;

            var fin = await call.Run(runtime);

            connectionIO.VerifyAll();
            functionIO.VerifyAll();
            dataIO.VerifyAll();
        }

        [Fact]
        public async Task CommitWithWait_is_called()
        {

            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out var connHandle);
            var functionIO = new Mock<SAPRfcFunctionIO>();
            var dataIO = new Mock<SAPRfcDataIO>();

            functionIO.SetupFunction("BAPI_TRANSACTION_COMMIT", connHandle, (h) =>
            {
                var structureHandle = new Mock<IStructureHandle>();
                dataIO.Setup(x => x.SetFieldValue(h, "X",
                        It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                    .Returns(Prelude.Right(Unit.Default));

                dataIO.Setup(x => x.GetStructure(h, "RETURN"))
                    .Returns(Prelude.Right(structureHandle.Object));
                dataIO.Setup(x =>
                        x.GetFieldValue<string>(structureHandle.Object,
                            It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                    .Returns(Prelude.Right(""));

            });

            var runtime = TestSAPRfcRuntime.New(settings =>
            {
                settings.RfcConnectionIO = connectionIO.Object;
                settings.RfcFunctionIO = functionIO.Object;
                settings.RfcDataIO = dataIO.Object;
            });

            var call = from conn in useConnection(NewConnection()
                    , commitAndWait)
                select conn;

            var fin = await call.Run(runtime);

            connectionIO.VerifyAll();
            functionIO.VerifyAll();
            dataIO.VerifyAll();


        }

        [Fact]
        public async Task Rollback_is_called()
        {

            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out var connHandle);
            var functionIO = new Mock<SAPRfcFunctionIO>();

            functionIO.SetupFunction("BAPI_TRANSACTION_ROLLBACK", connHandle, _=>{});

            var runtime = TestSAPRfcRuntime.New(settings =>
            {
                settings.RfcConnectionIO = connectionIO.Object;
                settings.RfcFunctionIO = functionIO.Object;
                settings.RfcDataIO = new Mock<SAPRfcDataIO>().Object;
            });

            var call = from conn in useConnection(NewConnection()
                    , rollback)
                select conn;

            var fin = await call.Run(runtime);

            connectionIO.VerifyAll();
            functionIO.VerifyAll();
        }


        // this test is failing on build server (but not locally)
        // disabled for now
        //[Fact]
        //public async Task Cancel_function_is_cancelled()
        //{

        //    var connectionIO = new Mock<SAPRfcConnectionIO>();
        //    connectionIO.SetupOpenConnection(out var connHandle);
        //    connectionIO.Setup(x => x.CancelConnection(connHandle.Object)).Returns(Prelude.Right(Prelude.unit));
        //    var functionIO = new Mock<SAPRfcFunctionIO>();

        //    functionIO.SetupFunction("MOCK", connHandle, f => { }, true);

        //    var runtime = TestSAPRfcRuntime.New(settings =>
        //    {
        //        settings.RfcConnectionIO = connectionIO.Object;
        //        settings.RfcFunctionIO = functionIO.Object;
        //        settings.RfcDataIO = new Mock<SAPRfcDataIO>().Object;
        //    });

        //    var call = from conn in useConnection(NewConnection(), c =>

        //            from fd in createFunction(c, "MOCK")
        //            from func in Prelude.fork(invokeFunction(c, fd))
        //            from _ in Prelude.cancel<TestSAPRfcRuntime>()
        //            from __ in func.ToAff()
        //            select c)
        //        select conn;

        //    var fin = await call.Run(runtime);
        //    fin.IfSucc(c => Assert.True(c.Disposed));

        //    connectionIO.VerifyAll();
        //    functionIO.VerifyAll();

        //}

    }
}