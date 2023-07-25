using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using LanguageExt;
using Moq;
using Xunit;

namespace YaNco.Core.Tests
{
    public class CreateRuntimeTests
    {

        [Fact]
        public void Runtime_Can_Be_Initialized_in_Parallel()
        {
            var connectionBuilder = new ConnectionBuilder(new Dictionary<string, string>());
            var connectionMock = new Mock<IConnection>();
            var emptyAttributes = new ConnectionAttributes("", "", "", "", "", "", "", "", "", "",
                    "", "", "", "", "", "", "", "", "", "", "", "", "", "");

            connectionMock.Setup(x => x.GetAttributes()).Returns(Prelude.RightAsync<RfcErrorInfo, ConnectionAttributes>(emptyAttributes));

            connectionBuilder.UseFactory((_, r) => Prelude.RightAsync<RfcErrorInfo, IConnection>(connectionMock.Object));
            var buildFunc = connectionBuilder.Build();

            var options = new ParallelOptions { MaxDegreeOfParallelism = int.MaxValue };
            Parallel.ForEach(Enumerable.Range(0, 1000), options, _ =>
            {
                buildFunc().Match(Assert.NotNull, l => l.Throw()).GetAwaiter().GetResult();

            });
        }
    }
}
