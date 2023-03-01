using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;

namespace SAPSystemTests
{
    public class AbapValueJsonConverter : JsonConverter<AbapValue>
    {
        public IFieldMapper FieldMapper;

        public AbapValueJsonConverter(IFieldMapper fieldMapper)
        {
            FieldMapper = fieldMapper;
        }

        public override AbapValue Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
            throw new NotImplementedException();


        public override void Write(
            Utf8JsonWriter writer,
            AbapValue value,
            JsonSerializerOptions options)
        {

            switch (value.FieldInfo.Type)
            {
                case RfcType.NUM:
                case RfcType.INT8:
                case RfcType._UTCLONG:
                case RfcType.UTCSECOND:
                case RfcType.UTCMINUTE:
                case RfcType.DTDAY:
                case RfcType._DTWEEK:
                case RfcType.DTMONTH:
                case RfcType.TSECOND:
                case RfcType.TMINUTE:
                case RfcType.CDAY:
                case RfcType.INT:
                case RfcType.INT2:
                case RfcType.INT1:
                    FieldMapper.FromAbapValue<long>(value)
                        .Match(writer.WriteNumberValue
                            , l => throw new Exception(l.Message));
                    break;
                case RfcType.DECF16:
                case RfcType.DECF34:
                    FieldMapper.FromAbapValue<decimal>(value)
                        .Match(writer.WriteNumberValue
                            , l => throw new Exception(l.Message));
                    break;
                case RfcType.FLOAT:
                    FieldMapper.FromAbapValue<float>(value)
                        .Match(writer.WriteNumberValue
                            , l => throw new Exception(l.Message));
                    break;
                case RfcType.XSTRING:
                case RfcType.BYTE:
                    FieldMapper.FromAbapValue<byte[]>(value)
                        .Match(b => writer.WriteBase64StringValue(new ReadOnlySpan<byte>(b))
                            , l => throw new Exception(l.Message));
                    break;

                case RfcType.TIME:
                case RfcType.DATE:
                case RfcType.CHAR:
                case RfcType.STRING:
                    FieldMapper.FromAbapValue<string>(value)
                        .Match(writer.WriteStringValue
                            , l => throw new Exception(l.Message));
                    break;

                case RfcType.BCD:
                    break;
                case RfcType.NULL:
                    break;
                case RfcType.ABAPOBJECT:
                    break;
                case RfcType.STRUCTURE:
                    FieldMapper.FromAbapValue<IDictionary<string, AbapValue>>(value)
                        .Match(
                            d => JsonSerializer.Serialize(writer, d, options),
                            l => throw new Exception(l.Message));
                    break;
                case RfcType.TABLE:
                    FieldMapper.FromAbapValue<IEnumerable<IDictionary<string, AbapValue>>>(value)
                        .Match(
                            ed => JsonSerializer.Serialize(writer, ed, options),
                            l => throw new Exception(l.Message));
                    break;
                case RfcType.XMLDATA:
                    break;
                case RfcType.BOX:
                    break;
                case RfcType.GENERIC_BOX:
                    break;
            }

        }
    }
}