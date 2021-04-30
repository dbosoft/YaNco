using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Dbosoft.YaNco.Internal;
using LanguageExt;
using Moq;
using Xunit;
using YaNco.Core.Tests.RfcMock;

namespace YaNco.Core.Tests
{
    public class ConnectionTests
    {
        [Fact]
        public async Task Connection_is_Opened()
        {
            var rfcRuntimeMock = new Mock<IRfcRuntime>();
            var handleMock = new Mock<IConnectionHandle>();

            rfcRuntimeMock.Setup(x =>
                    x.OpenConnection(new Dictionary<string, string>()))
                .Returns(Prelude.Right(handleMock.Object));

            var connection = await Connection.Create(new Dictionary<string, string>(), 
                rfcRuntimeMock.Object)
                .IfLeft(l => throw new Exception(l.Message));

            rfcRuntimeMock.Verify();

        }
        
        [Fact]
        public async Task Function_is_created()
        {
            var rfcRuntimeMock = new Mock<IRfcRuntime>();
            var descMock = new Mock<IFunctionDescriptionHandle>();
            var funcMock = new Mock<IFunctionHandle>();
            var connMock = new Mock<IConnectionHandle>();

            rfcRuntimeMock.Setup(x =>
                    x.OpenConnection(new Dictionary<string, string>()))
                .Returns(Prelude.Right(connMock.Object));


            rfcRuntimeMock.Setup(x =>
                    x.GetFunctionDescription(
                        It.IsAny<IConnectionHandle>(),
                        "RFC_PING"))
                .Returns(Prelude.Right(descMock.Object));

            rfcRuntimeMock.Setup(x =>
                    x.CreateFunction(
                        descMock.Object))
                .Returns(Prelude.Right(funcMock.Object));


            var connection = await Connection.Create(new Dictionary<string, string>(),
                    rfcRuntimeMock.Object)
                .IfLeft(l => throw new Exception(l.Message));

            await connection.CreateFunction("RFC_PING")
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.Verify();

        }
    }
}
