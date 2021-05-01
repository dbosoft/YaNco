using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dbosoft.YaNco;
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
            rfcRuntimeMock.SetupOpenConnection(out _);

            await rfcRuntimeMock.CreateConnection();

            rfcRuntimeMock.VerifyAll();

        }

        [Fact]
        public async Task Function_is_created()
        {
            var rfcRuntimeMock = new Mock<IRfcRuntime>()
                .SetupOpenConnection(out _)
                .SetupGetFunctionDescription("RFC_PING", out var descHandle)
                .SetupGetFunction(descHandle, out _);

            await (await rfcRuntimeMock.CreateConnection())
                .CreateFunction("RFC_PING")
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.VerifyAll();

        }

        [Fact]
        public async Task Commit_is_called()
        {

            var rfcRuntimeMock = new Mock<IRfcRuntime>()
                .SetupOpenConnection(out var connHandle)
                .SetupFunction("BAPI_TRANSACTION_COMMIT", connHandle, (r,h) =>
                {
                    var structureHandle = new Mock<IStructureHandle>();

                    r.Setup(x => x.GetStructure(h, "RETURN"))
                        .Returns(Prelude.Right(structureHandle.Object));
                    r.Setup(x =>
                            x.GetFieldValue<string>(structureHandle.Object,
                                It.IsAny<Func<Either<RfcErrorInfo, RfcFieldInfo>>>()))
                        .Returns(Prelude.Right(""));
                });

            await (await rfcRuntimeMock.CreateConnection())
                .Commit()
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.VerifyAll();

        }
    }
}
