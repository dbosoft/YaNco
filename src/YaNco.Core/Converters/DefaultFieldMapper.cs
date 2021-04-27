using System;
using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public class DefaultFieldMapper : IFieldMapper
    {
        private readonly IRfcConverterResolver _converterResolver;

        public DefaultFieldMapper(IRfcConverterResolver converterResolver)
        {
            _converterResolver = converterResolver;
        }

        public Either<RfcErrorInfo, Unit> SetField<T>(T value, FieldMappingContext context)
        {
            AbapValue abapValue = null;

            foreach (var converter in _converterResolver.GetToRfcConverters<T>(context.FieldInfo.Type))
            {
                var result = converter.ConvertFrom(value, context.FieldInfo)();
                if(result.IsFaulted)
                    continue;
                result.IfSucc(v => abapValue = v);
                break;

            }

            if (abapValue == null)
                return new RfcErrorInfo(RfcRc.RFC_CONVERSION_FAILURE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, "",
                    $"Converting from type {typeof(T)} to abap type {context.FieldInfo.Type} is not supported.",
                    "", "E", "", "", "", "", "");

            switch (abapValue)
            {
                case AbapIntValue abapIntValue:
                    return context.RfcRuntime.SetInt(context.Handle, context.FieldInfo.Name, abapIntValue.Value);
                case AbapLongValue abapLongValue:
                    return context.RfcRuntime.SetLong(context.Handle, context.FieldInfo.Name, abapLongValue.Value);
                case AbapByteValue abapByteValue:
                    return context.RfcRuntime.SetBytes(context.Handle, context.FieldInfo.Name, abapByteValue.Value,
                        abapByteValue.Value.LongLength);
                case AbapStringValue abapStringValue:
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (context.FieldInfo.Type)
                    {
                        case RfcType.DATE:
                            return context.RfcRuntime.SetDateString(context.Handle, context.FieldInfo.Name,
                                abapStringValue.Value);
                        case RfcType.TIME:
                            return context.RfcRuntime.SetTimeString(context.Handle, context.FieldInfo.Name,
                                abapStringValue.Value);
                        default:
                            return context.RfcRuntime.SetString(context.Handle, context.FieldInfo.Name, abapStringValue.Value);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(abapValue));

            }
        }

        public Either<RfcErrorInfo, T> GetField<T>(FieldMappingContext context)
        {
            return context.Apply(c =>
            {
                switch (context.FieldInfo.Type)
                {
                    case RfcType.DATE:
                        return context.RfcRuntime.GetDateString(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapStringValue(context.FieldInfo, v));
                    case RfcType.TIME:
                        return context.RfcRuntime.GetTimeString(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapStringValue(context.FieldInfo, v));
                    case RfcType.CHAR:
                    case RfcType.NUM:
                    case RfcType.STRING:
                    case RfcType.BCD:
                    case RfcType.FLOAT:
                    case RfcType.DECF16:
                    case RfcType.DECF34:
                        return context.RfcRuntime.GetString(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapStringValue(context.FieldInfo, v));
                    case RfcType.INT:
                    case RfcType.INT2:
                    case RfcType.INT1:
                        return context.RfcRuntime.GetInt(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapIntValue(context.FieldInfo, v));
                    case RfcType.INT8:
                        return context.RfcRuntime.GetLong(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapLongValue(context.FieldInfo, v));
                    case RfcType.BYTE:
                    case RfcType.XSTRING:
                        return context.RfcRuntime.GetBytes(context.Handle, context.FieldInfo.Name).Map(v =>
                            (AbapValue) new AbapByteValue(context.FieldInfo, v));

                    default:
                        throw new NotSupportedException(
                            $"Reading a field of RfcType {context.FieldInfo.Type} is not supported for this method.");
                }
            }).Bind(abapValue =>
            {
                T value = default;
                foreach (var converter in _converterResolver.GetFromRfcConverters<T>(context.FieldInfo.Type, abapValue.GetType()))
                {
                    var result = converter.ConvertTo(abapValue)();
                    if (result.IsFaulted)
                        continue;
                    result.IfSucc(v => value = v);
                    break;
                }

                if (value == null)
                    return new RfcErrorInfo(RfcRc.RFC_CONVERSION_FAILURE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, "",
                        $"Converting from abap type {context.FieldInfo.Type} to type {typeof(T)} is not supported.",
                        "", "E", "", "", "", "", "");

                return Prelude.Right<RfcErrorInfo,T>(value);
            });
        }
    }
}