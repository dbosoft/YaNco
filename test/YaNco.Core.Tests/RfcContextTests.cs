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
    public class RfcContextTests
    {

        [Fact]
        public async Task Connection_Is_Created()
        {
            var connectionIO = new Mock<SAPRfcConnectionIO>();
            connectionIO.SetupOpenConnection(out _);
            var connFunc = new ConnectionBuilder(new Dictionary<string, string>())
                .ConfigureRuntime(c => c.UseSettingsFactory((l, m, o) => 
                    new SAPRfcRuntimeSettings(l, m, o)
                {
                    RfcConnectionIO = connectionIO.Object
                })).Build();


            using (var rfcContext = new RfcContext(connFunc))
            {
                await rfcContext.GetConnection().IfLeft(l => l.Throw());

            }

            connectionIO.VerifyAll();


        }

        //[Fact]
        //public async Task Function_Is_Called()
        //{
        //    var rfcRuntimeMock = new Mock<IRfcRuntime>()
        //        .SetupOpenConnection(out var connHandle);

        //    rfcRuntimeMock.SetupFunction("A_FUNC", connHandle,
        //        (r, h) =>
        //        {
        //            r.Setup(x => x.GetFieldValue<string>(h, It.IsAny<Func<Either<RfcError, RfcFieldInfo>>>()
        //                ))
        //                .Returns("VALUE");
        //        });

        //    using (var rfcContext = new RfcContext(rfcRuntimeMock.CreateConnectionFactory()))
        //    {
        //        await rfcContext.CallFunction("A_FUNC",
        //                f => f.GetField<string>("FIELD"),
        //                f => f)
        //            .IfLeft(l => { l.Throw(); });

        //    }

        //    rfcRuntimeMock.VerifyAll();


        //}

        //[Fact]
        //public async Task Ping_Is_Called()
        //{
        //    var rfcRuntimeMock = new Mock<IRfcRuntime>()
        //        .SetupOpenConnection(out var connHandle);

        //    rfcRuntimeMock.SetupFunction("RFC_PING", connHandle,
        //        (r, h) => { });

        //    using (var rfcContext = new RfcContext(rfcRuntimeMock.CreateConnectionFactory()))
        //    {
        //        await rfcContext.Ping()
        //            .IfLeft(l => { l.Throw(); });

        //        rfcRuntimeMock.VerifyAll();


        //        (await rfcContext.PingAsync())
        //            .IfLeft(l => { l.Throw(); });

        //    }


        //}
    }
}