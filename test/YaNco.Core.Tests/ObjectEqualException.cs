using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace YaNco.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public class ObjectEqualException : XunitException
    {
        private readonly string message;

        public ObjectEqualException(string message)
            : base("Assert.Equal() Failure")
        {
            this.message = message;
        }

        public override string Message => message;
    }
}