using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dbosoft.YaNco;
using LanguageExt;

namespace CreateSalesOrder
{
    [ExcludeFromCodeCoverage]
    internal static class TableReturnExtensions
    {
        /// <summary>
        /// This methods extracts the value of a RETURN table (BAPIRET or BAPIRET2) and processes it's value as
        /// left value if return contains a non-successful result (abort or error). 
        /// This method accepts a EitherAsync with any right value.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A <see cref="EitherAsync{RfcErrorInfo,IFunction}"/> with the function as right value or the left value.</returns>
        public static Either<RfcErrorInfo, IFunction> HandleReturnTable(this Either<RfcErrorInfo, IFunction> self)
        {
            return self.Bind(f => (               
                from retTab in f.GetTable("RETURN")
                from messages in retTab.Rows.Map( r => 
                    from type in r.GetField<string>("TYPE")
                    from id in r.GetField<string>("ID")
                    from number in r.GetField<string>("NUMBER")
                    from message in r.GetField<string>("MESSAGE")
                    from v1 in r.GetField<string>("MESSAGE_V1")
                    from v2 in r.GetField<string>("MESSAGE_V2")
                    from v3 in r.GetField<string>("MESSAGE_V3")
                    from v4 in r.GetField<string>("MESSAGE_V4")
                    select new ReturnData { Type=type, Id= id, Number= number, Message= message, 
                        MessageV1= v1, MessageV2 = v2, MessageV3 = v3, MessageV4 = v4}
                ).Traverse(l=>l)            
                from _ in ErrorOrResult(f, messages)
                select f));

        }

        private class ReturnData
        {
            public string Type { get; set; }
            public string Id { get; set; }
            public string Number { get; set; }
            public string Message { get; set; }

            public string MessageV1 { get; set; }
            public string MessageV2 { get; set; }
            public string MessageV3 { get; set; }
            public string MessageV4 { get; set; }



        }

        private static Either<RfcErrorInfo, TResult> ErrorOrResult<TResult>(TResult result, IEnumerable<ReturnData> messages)
        {
            var messagesArray = messages as ReturnData[] ?? messages.ToArray();

            if (!messagesArray.Any(x => x.Type.Contains('E') || x.Type.Contains('A'))) 
                return result;

            var failedMessage = messagesArray.FirstOrDefault(x => x.Type.Contains('A'))
                                ?? messagesArray.FirstOrDefault(x => x.Type.Contains('E'));

            return new RfcErrorInfo(RfcRc.RFC_ABAP_MESSAGE, RfcErrorGroup.ABAP_APPLICATION_FAILURE, "",
                failedMessage?.Message, failedMessage?.Id,
                failedMessage?.Type, failedMessage?.Number,
                failedMessage?.MessageV1, failedMessage?.MessageV2,
                failedMessage?.MessageV3, failedMessage?.MessageV4);
        }


    }
}