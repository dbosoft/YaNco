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

    public static class IOMockExtensions
    {

        public static Mock<SAPRfcConnectionIO> SetupOpenConnection(this Mock<SAPRfcConnectionIO> mock, out Mock<IConnectionHandle> handle)
        {
            handle = new Mock<IConnectionHandle>();

            mock.Setup(x =>
                    x.OpenConnection(new Dictionary<string, string>()))
                .Returns(Prelude.Right(handle.Object));
            mock.Setup(x => x.GetConnectionAttributes(It.IsAny<IConnectionHandle>()))
                .Returns(Prelude.Right(ConnectionAttributes.Empty()));

            return mock;
        }

        public static Mock<SAPRfcFunctionIO> SetupGetFunctionDescription(this Mock<SAPRfcFunctionIO> mock, string functionName, out Mock<IFunctionDescriptionHandle> handle)
        {
            handle = new Mock<IFunctionDescriptionHandle>();

            mock.Setup(x =>
                    x.GetFunctionDescription(
                        It.IsAny<IConnectionHandle>(),
                        functionName))
                .Returns(Prelude.Right(handle.Object));

            return mock;
        }

        public static Mock<SAPRfcFunctionIO> SetupGetFunction(this Mock<SAPRfcFunctionIO> mock, Mock<IFunctionDescriptionHandle> descHandle, out Mock<IFunctionHandle> handle)
        {
            handle = new Mock<IFunctionHandle>();

            mock.Setup(x =>
                    x.CreateFunction(
                        descHandle.Object))
                .Returns(Prelude.Right(handle.Object));


            return mock;
        }

        public static Mock<SAPRfcFunctionIO> SetupFunction(this Mock<SAPRfcFunctionIO> mock, string functionName, Mock<IConnectionHandle> connHandle, 
            Action<IFunctionHandle> functionBuilder, bool delay = false)
        {
            mock.SetupGetFunctionDescription(functionName, out var descHandle)
                .SetupGetFunction(descHandle, out var funcHandle);

            mock.Setup(x =>
                    x.Invoke(connHandle.Object, funcHandle.Object))
                .Returns(() =>
                {
                    if (delay)
                        Thread.Sleep(2000);
                    return Prelude.Right(Unit.Default);
                });

            functionBuilder(funcHandle.Object);

            return mock;
        }
    }
}