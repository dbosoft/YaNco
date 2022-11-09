using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace YaNco.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public class ObjectEqualException : AssertActualExpectedException
    {
        private readonly string message;

        public ObjectEqualException(object expected, object actual, string message)
            : base(expected, actual, "Assert.Equal() Failure")
        {
            this.message = message;
        }

        public override string Message => message;
    }
}