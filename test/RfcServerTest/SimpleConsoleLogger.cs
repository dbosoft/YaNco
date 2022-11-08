using System.Text;
using Dbosoft.YaNco;
using Newtonsoft.Json;

namespace RfcServerTest
{
    public class SimpleConsoleLogger : ILogger
    {
        public void LogException(Exception exception, string message)
        {
            Console.WriteLine($"{message} - Exception: {exception}");
        }

        public void LogException(Exception exception)
        {
            LogException(exception, "");
        }

        public void LogTrace(string message, object data)
        {
           //Console.WriteLine($"TRACE\t{message}{ObjectToString(data)}");
        }

        public void LogError(string message, object data)
        {
            Console.WriteLine($"ERROR\t{message}{ObjectToString(data)}");
        }

        public void LogDebug(string message, object data)
        {
            if (data is RfcErrorInfo { Key: "RFC_TABLE_MOVE_EOF" })
                return;

            Console.WriteLine($"DEBUG\t{message}{ObjectToString(data)}");
        }

        public void LogTrace(string message)
        {
            LogTrace(message, null);
        }

        public void LogDebug(string message)
        {
            LogDebug(message, null);
        }

        public void LogError(string message)
        {
            LogError(message, null);
        }

        public static string ObjectToString(object valueObject)
        {
            var dataString = new StringBuilder();
            if (valueObject == null)
                return "";

            dataString.Append(", Data: ");

            dataString.Append(JsonConvert.SerializeObject(valueObject, new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>(new[]{new HandleToStringJsonConverter() })
            }));

            return dataString.ToString();

        }

        class HandleToStringJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                if(objectType.BaseType  != null 
                   && objectType.BaseType.FullName != null 
                   && (objectType.BaseType.FullName.StartsWith("Dbosoft.SAP.NWRfc.Native.HandleBase") 
                   || objectType.BaseType.FullName.StartsWith("Dbosoft.SAP.NWRfc.Native.DataContainerBase")))
                    return true;
                return false;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var typeName = value.GetType().Name;                
                writer.WriteValue($"{typeName}<{value}>");
            }

            public override bool CanRead
            {
                get { return false; }
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}