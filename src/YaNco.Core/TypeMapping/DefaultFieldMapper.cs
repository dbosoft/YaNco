using System;
using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

/// <summary>
/// THis is the default field mapper that is used by to map fields from and to SAP RFC
/// </summary>
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
            return abapValue switch
            {
                AbapIntValue abapIntValue => context.IO.SetInt(context.Handle, context.FieldInfo.Name,
                    abapIntValue.Value),
                AbapLongValue abapLongValue => context.IO.SetLong(context.Handle, context.FieldInfo.Name,
                    abapLongValue.Value),
                AbapByteValue abapByteValue => context.IO.SetBytes(context.Handle, context.FieldInfo.Name,
                    abapByteValue.Value, abapByteValue.Value.LongLength),
                AbapStringValue abapStringValue =>
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    context.FieldInfo.Type switch
                    {
                        RfcType.DATE => context.IO.SetDateString(context.Handle, context.FieldInfo.Name,
                            abapStringValue.Value),
                        RfcType.TIME => context.IO.SetTimeString(context.Handle, context.FieldInfo.Name,
                            abapStringValue.Value),
                        _ => context.IO.SetString(context.Handle, context.FieldInfo.Name, abapStringValue.Value)
                    },
                _ => throw new ArgumentOutOfRangeException(nameof(abapValue))
            };
        });

    }

    public Either<RfcError, T> GetField<T>(FieldMappingContext context)
    {
        return context.Apply(_ =>
        {
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            // ReSharper disable once SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
            return context.FieldInfo.Type switch
            {
                RfcType.DATE => context.IO.GetDateString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.TIME => context.IO.GetTimeString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.CHAR => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.NUM => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.STRING => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.BCD => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.FLOAT => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.DECF16 => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.DECF34 => context.IO.GetString(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapStringValue(context.FieldInfo, v)),
                RfcType.INT => context.IO.GetInt(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapIntValue(context.FieldInfo, v)),
                RfcType.INT2 => context.IO.GetInt(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapIntValue(context.FieldInfo, v)),
                RfcType.INT1 => context.IO.GetInt(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapIntValue(context.FieldInfo, v)),
                RfcType.INT8 => context.IO.GetLong(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapLongValue(context.FieldInfo, v)),
                RfcType.BYTE => context.IO.GetBytes(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapByteValue(context.FieldInfo, v)),
                RfcType.XSTRING => context.IO.GetBytes(context.Handle, context.FieldInfo.Name)
                    .Map(v => (AbapValue)new AbapByteValue(context.FieldInfo, v)),
                RfcType.STRUCTURE => context.IO.GetStructure(context.Handle, context.FieldInfo.Name)
                    .Map(handle => (IStructure)new Structure(handle, context.IO))
                    .Bind(s => s.ToDictionary())
                    .Map(d => (AbapValue)new AbapStructureValues(context.FieldInfo, d)),
                RfcType.TABLE => context.IO.GetTable(context.Handle, context.FieldInfo.Name)
                    .Map(handle => (ITable)new Table(handle, context.IO))
                    .MapStructure(d => d.ToDictionary())
                    .Map(tr => (AbapValue)new AbapTableValues(context.FieldInfo, tr)),
                _ => throw new NotSupportedException(
                    $"Reading a field of RfcType {context.FieldInfo.Type} is not supported for this method.")
            };
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

}