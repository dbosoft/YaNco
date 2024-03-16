using System.Diagnostics.CodeAnalysis;
using Xunit.Sdk;

namespace YaNco.Core.Tests
{
    [ExcludeFromCodeCoverage]
    public class ObjectEqualException : XunitException
    {
        public ObjectEqualException(string message)
            : base("Assert.Equal() Failure")
        {
            Message = message;
        }

        public override string Message { get; }
    }
}