using System;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

public class DefaultFieldMapper : IFieldMapper
{
    private readonly IRfcConverterResolver _converterResolver;

    public DefaultFieldMapper(IRfcConverterResolver converterResolver)
    {
        _converterResolver = converterResolver;
    }

    public Either<RfcError, Unit> SetField<T>(T value, FieldMappingContext context)
    {
        return ToAbapValue(value, context.FieldInfo).Bind(abapValue =>
        {
            switch (abapValue)
            {
                case AbapIntValue abapIntValue:
                    return context.IO.SetInt(context.Handle, context.FieldInfo.Name, abapIntValue.Value);
                case AbapLongValue abapLongValue:
                    return context.IO.SetLong(context.Handle, context.FieldInfo.Name, abapLongValue.Value);
                case AbapByteValue abapByteValue:
                    return context.IO.SetBytes(context.Handle, context.FieldInfo.Name, abapByteValue.Value,
                        abapByteValue.Value.LongLength);
                case AbapStringValue abapStringValue:
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    return context.FieldInfo.Type switch
                    {
                        RfcType.DATE => context.IO.SetDateString(context.Handle, context.FieldInfo.Name,
                            abapStringValue.Value),
                        RfcType.TIME => context.IO.SetTimeString(context.Handle, context.FieldInfo.Name,
                            abapStringValue.Value),
                        _ => context.IO.SetString(context.Handle, context.FieldInfo.Name, abapStringValue.Value)
                    };

                default:
                    throw new ArgumentOutOfRangeException(nameof(abapValue));
            }
        });

    }

    public Either<RfcError, T> GetField<T>(FieldMappingContext context)
    {
        return context.Apply(_ =>
        {
            switch (context.FieldInfo.Type)
            {
                case RfcType.DATE:
                    return context.IO.GetDateString(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapStringValue(context.FieldInfo, v));
                case RfcType.TIME:
                    return context.IO.GetTimeString(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapStringValue(context.FieldInfo, v));
                case RfcType.CHAR:
                case RfcType.NUM:
                case RfcType.STRING:
                case RfcType.BCD:
                case RfcType.FLOAT:
                case RfcType.DECF16:
                case RfcType.DECF34:
                    return context.IO.GetString(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapStringValue(context.FieldInfo, v));
                case RfcType.INT:
                case RfcType.INT2:
                case RfcType.INT1:
                    return context.IO.GetInt(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapIntValue(context.FieldInfo, v));
                case RfcType.INT8:
                    return context.IO.GetLong(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapLongValue(context.FieldInfo, v));
                case RfcType.BYTE:
                case RfcType.XSTRING:
                    return context.IO.GetBytes(context.Handle, context.FieldInfo.Name).Map(v =>
                        (AbapValue) new AbapByteValue(context.FieldInfo, v));
                case RfcType.STRUCTURE:
                    return context.IO.GetStructure(context.Handle, context.FieldInfo.Name)
                        .Map(handle => (IStructure)new Structure(handle, context.IO))
                        .Bind(s => s.ToDictionary())
                        .Map(d => (AbapValue)new AbapStructureValues(context.FieldInfo, d));
                case RfcType.TABLE:
                    return context.IO.GetTable(context.Handle, context.FieldInfo.Name)
                        .Map(handle => (ITable)new Table(handle, context.IO))
                        .MapStructure(d => d.ToDictionary())
                        .Map(tr => (AbapValue)new AbapTableValues(context.FieldInfo, tr));

                default:
                    throw new NotSupportedException(
                        $"Reading a field of RfcType {context.FieldInfo.Type} is not supported for this method.");
            }
        }).Bind(FromAbapValue<T>);
    }

    public Either<RfcError, T> FromAbapValue<T>(AbapValue abapValue)
    {
        if (abapValue is T tv)
            return tv;

        T value = default;
        foreach (var converter in _converterResolver.GetFromRfcConverters<T>(abapValue.FieldInfo.Type, abapValue.GetType()))
        {
            var result = converter.ConvertTo(abapValue)();
            if (result.IsFaulted)
                continue;
            result.IfSucc(v => value = v);
            break;
        }

        if (value == null)
            return new RfcErrorInfo(RfcRc.RFC_CONVERSION_FAILURE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, "",
                $"Converting from abap type {abapValue.FieldInfo.Type} to type {typeof(T)} is not supported.",
                "", "E", "", "", "", "", "").ToRfcError();

        return Prelude.Right<RfcError, T>(value);
    }

    public Either<RfcError, AbapValue> ToAbapValue<T>(T value, RfcFieldInfo fieldInfo)
    {
        AbapValue abapValue = null;

        if (value is AbapValue av)
            return av;

        foreach (var converter in _converterResolver.GetToRfcConverters<T>(fieldInfo.Type))
        {
            var result = converter.ConvertFrom(value, fieldInfo)();
            if (result.IsFaulted)
                continue;
            result.IfSucc(v => abapValue = v);
            break;

        }

        if (abapValue == null)
            return new RfcErrorInfo(RfcRc.RFC_CONVERSION_FAILURE, RfcErrorGroup.EXTERNAL_APPLICATION_FAILURE, "",
                $"Converting from type {typeof(T)} to abap type {fieldInfo.Type} is not supported.",
                "", "E", "", "", "", "", "").ToRfcError();

        return abapValue;
    }

    public Either<RfcError, Unit> SetFieldValue<T>(SAPRfcDataIO rfcRuntime, IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        return func().Bind(fieldInfo => 
            SetField(value, new FieldMappingContext(rfcRuntime, handle, fieldInfo)));

    }

    public Either<RfcError, T> GetFieldValue<T>(SAPRfcDataIO rfcRuntime, IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func)
    {
        return func().Bind(fieldInfo => 
            GetField<T>(new FieldMappingContext(rfcRuntime, handle, fieldInfo)));
    }
}