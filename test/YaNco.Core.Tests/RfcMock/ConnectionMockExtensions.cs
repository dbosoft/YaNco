using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Moq;

namespace YaNco.Core.Tests.RfcMock
{

    [ExcludeFromCodeCoverage]
    public static class ServerMockExtensions
    {
        public static async Task<IRfcServer> CreateServer(this Mock<IRfcRuntime> mock)
        {
            return await RfcServer.Create(new Dictionary<string, string>(),
                    mock.Object)
                .IfLeft(l => throw new Exception(l.Message));

        }

        public static Mock<IRfcRuntime> SetupCreateServer(this Mock<IRfcRuntime> mock, out Mock<IRfcServerHandle> handle)
        {
            handle = new Mock<IRfcServerHandle>();

            mock.Setup(x =>
                    x.CreateServer(new Dictionary<string, string>()))
                .Returns(Prelude.Right(handle.Object));

            return mock;
        }

    }

    public static class ConnectionMockExtensions
    {
        public static async Task<IConnection> CreateConnection(this Mock<IRfcRuntime> mock)
        {
            return await CreateConnectionFactory(mock)()
                .IfLeft(l => throw new Exception(l.Message));

        }

        public static Func<EitherAsync<RfcError, IConnection>>
            CreateConnectionFactory(this Mock<IRfcRuntime> mock)
        {
            return () => Connection.Create(new Dictionary<string, string>(),
                mock.Object);

        }

        public static Mock<IRfcRuntime> SetupOpenConnection(this Mock<IRfcRuntime> mock, out Mock<IConnectionHandle> handle)
        {
            handle = new Mock<IConnectionHandle>();

            mock.Setup(x =>
                    x.OpenConnection(new Dictionary<string, string>()))
                .Returns(Prelude.Right(handle.Object));

            return mock;
        }

        public static Mock<IRfcRuntime> SetupGetFunctionDescription(this Mock<IRfcRuntime> mock, string functionName, out Mock<IFunctionDescriptionHandle> handle)
        {
            handle = new Mock<IFunctionDescriptionHandle>();

            mock.Setup(x =>
                    x.GetFunctionDescription(
                        It.IsAny<IConnectionHandle>(),
                        functionName))
                .Returns(Prelude.Right(handle.Object));

            return mock;
        }

        public static Mock<IRfcRuntime> SetupGetFunction(this Mock<IRfcRuntime> mock, Mock<IFunctionDescriptionHandle> descHandle, out Mock<IFunctionHandle> handle)
        {
            handle = new Mock<IFunctionHandle>();

            mock.Setup(x =>
                    x.CreateFunction(
                        descHandle.Object))
                .Returns(Prelude.Right(handle.Object));


            return mock;
        }        
        
        public static Mock<IRfcRuntime> SetupFunction(this Mock<IRfcRuntime> mock, string functionName, Mock<IConnectionHandle> connHandle, Action<Mock<IRfcRuntime>, IFunctionHandle> functionBuilder, bool delay = false)
        {
            mock.SetupGetFunctionDescription(functionName, out var descHandle)
                .SetupGetFunction(descHandle, out var funcHandle);

            mock.Setup(x =>
                    x.Invoke(connHandle.Object, funcHandle.Object))
                .Returns(() =>
                {
                    if(delay)
                        Thread.Sleep(2000);
                    return Prelude.Right(Unit.Default);
                });

            functionBuilder(mock,funcHandle.Object);

            return mock;
        }
    }
}