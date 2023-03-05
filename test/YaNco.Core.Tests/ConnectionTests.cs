using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Moq;
using Xunit;
using YaNco.Core.Tests.RfcMock;

namespace YaNco.Core.Tests
{
    
    [ExcludeFromCodeCoverage]

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
                .SetupFunction("BAPI_TRANSACTION_COMMIT", connHandle, (r, h) =>
                {
                    var structureHandle = new Mock<IStructureHandle>();

                    r.Setup(x => x.GetStructure(h, "RETURN"))
                        .Returns(Prelude.Right(structureHandle.Object));
                    r.Setup(x =>
                            x.GetFieldValue<string>(structureHandle.Object,
                                It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                        .Returns(Prelude.Right(""));
                });

            await (await rfcRuntimeMock.CreateConnection())
                .Commit()
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.VerifyAll();

        }

        [Fact]
        public async Task CommitWithWait_is_called()
        {

            var rfcRuntimeMock = new Mock<IRfcRuntime>()
                .SetupOpenConnection(out var connHandle)
                .SetupFunction("BAPI_TRANSACTION_COMMIT", connHandle, (r, h) =>
                {
                    var structureHandle = new Mock<IStructureHandle>();
                    r.Setup(x => x.SetFieldValue(h, "X",
                            It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                        .Returns(Prelude.Right(Unit.Default));

                    r.Setup(x => x.GetStructure(h, "RETURN"))
                        .Returns(Prelude.Right(structureHandle.Object));
                    r.Setup(x =>
                            x.GetFieldValue<string>(structureHandle.Object,
                                It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()))
                        .Returns(Prelude.Right(""));
                });

            await (await rfcRuntimeMock.CreateConnection())
                .CommitAndWait()
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.VerifyAll();

        }

        [Fact]
        public async Task Rollback_is_called()
        {

            var rfcRuntimeMock = new Mock<IRfcRuntime>()
                .SetupOpenConnection(out var connHandle)
                .SetupFunction("BAPI_TRANSACTION_ROLLBACK", connHandle, (r, h) => { });

            await (await rfcRuntimeMock.CreateConnection())
                .Rollback()
                .IfLeft(l => throw new Exception(l.Message));


            rfcRuntimeMock.VerifyAll();

        }


        [Fact]
        public async Task Cancel_function_is_cancelled()
        {
            var rfcRuntimeMock = new Mock<IRfcRuntime>()
                .SetupOpenConnection(out var connHandle)
                .SetupFunction("MOCK", connHandle, (r, h) => { }, true);

            rfcRuntimeMock.Setup(r => r.CancelConnection(connHandle.Object))
                .Returns(Prelude.Right(new Unit()));

            var conn = await rfcRuntimeMock.CreateConnection()
                .Map(c =>
                    from fd in c.CreateFunction("MOCK")
                    from _ in c.InvokeFunction(fd)
                    from __ in c.Cancel()
                    select c);

            await conn.IfRight(c => Assert.True(c.Disposed));

            rfcRuntimeMock.VerifyAll();
        }

    }
}