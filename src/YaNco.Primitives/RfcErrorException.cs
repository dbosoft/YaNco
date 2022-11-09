using System;
using System.Runtime.Serialization;

namespace Dbosoft.YaNco
{
    [Serializable]
    public class RfcErrorException : Exception
    {
        public readonly RfcErrorInfo ErrorInfo;

        public RfcErrorException(RfcErrorInfo errorInfo) : base(errorInfo.Message)
        {
            ErrorInfo = errorInfo;
        }
        

        public RfcErrorException(RfcErrorInfo errorInfo, Exception inner) : base(errorInfo.Message, inner)
        {
            ErrorInfo = errorInfo;
        }

        protected RfcErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}