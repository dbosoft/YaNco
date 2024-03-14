//using System;
//using System.Threading;
//using System.Threading.Tasks;
//using Dbosoft.YaNco;
//using LanguageExt;
//using Moq;
//using Xunit;
//using YaNco.Core.Tests.RfcMock;

//namespace YaNco.Core.Tests
//{
//    public class RfcServerTests
//    {
//        [Fact]
//        public async Task Server_Is_Created()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out _);

//            await rfcRuntimeMock.CreateServer();

//            rfcRuntimeMock.VerifyAll();

//        }

//        [Fact]
//        public async Task Server_Is_Started()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out var handle);
//            rfcRuntimeMock.Setup(x =>
//                    x.LaunchServer(handle.Object))
//                .Returns(Unit.Default);


//            var rfcServer = await rfcRuntimeMock.CreateServer();
//            await rfcServer.Start()
//                .IfLeft(l => throw new Exception(l.Message));

//            rfcRuntimeMock.VerifyAll();

//        }

//        [Fact]
//        public async Task Server_Is_Stopped()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out var handle);
//            rfcRuntimeMock.Setup(x =>
//                    x.ShutdownServer(handle.Object, 0))
//                .Returns(Unit.Default);


//            var rfcServer = await rfcRuntimeMock.CreateServer();
//            await rfcServer.Stop()
//                .IfLeft(l => throw new Exception(l.Message));

//            rfcRuntimeMock.VerifyAll();

//        }

//        [Fact]
//        public async Task ConnectionPlaceHolder_Does_Nothing()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out var handle);
//            var rfcServer = await rfcRuntimeMock.CreateServer();

//            var conn =
//                (await rfcServer.OpenClientConnection().ToEither())
//                .RightAsEnumerable().FirstOrDefault();

//            Assert.Same(rfcRuntimeMock.Object, conn.RfcRuntime);
//            Assert.False(conn.Disposed);
//            Assert.True((await conn.GetAttributes().ToEither()).IsLeft);
//            Assert.True((await conn.CreateFunction("").ToEither()).IsLeft);
//            Assert.True((await conn.Cancel().ToEither()).IsLeft);
//            Assert.True((await conn.Commit().ToEither()).IsLeft);
//            Assert.True((await conn.CommitAndWait().ToEither()).IsLeft);
//            Assert.True((await conn.Rollback().ToEither()).IsLeft);
//            Assert.True((await conn.InvokeFunction(
//                new Mock<IFunction>().Object).ToEither()).IsLeft);

//            Assert.True((await conn.Commit(CancellationToken.None).ToEither()).IsLeft);
//            Assert.True((await conn.CommitAndWait(CancellationToken.None).ToEither()).IsLeft);
//            Assert.True((await conn.Rollback(CancellationToken.None).ToEither()).IsLeft);
//            Assert.True((await conn.InvokeFunction(
//                new Mock<IFunction>().Object, CancellationToken.None).ToEither()).IsLeft);

//            conn.Dispose();
//            Assert.True(conn.Disposed);


//        }

//        [Fact]
//        public async Task Server_Is_Invalid_After_Disposal()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out var handle);

//            rfcRuntimeMock.Setup(x =>
//                    x.LaunchServer(handle.Object))
//                .Returns(Unit.Default);

//            var rfcServer = await rfcRuntimeMock.CreateServer();
//            rfcServer.Dispose();
//            rfcServer.Start();

//            rfcRuntimeMock
//                .Verify(x=>x.LaunchServer(handle.Object), Times.Never());


//        }

//        [Fact]
//        public async Task Server_Never_Throws()
//        {
//            var rfcRuntimeMock = new Mock<IRfcRuntime>();
//            rfcRuntimeMock.SetupCreateServer(out var handle);

//            rfcRuntimeMock.Setup(x =>
//                    x.LaunchServer(handle.Object))
//                .Throws<Exception>();

//            var rfcServer = await rfcRuntimeMock.CreateServer();

//            rfcServer.Start();


//        }
//    }
//}