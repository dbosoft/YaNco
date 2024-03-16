using System;
using System.Collections.Generic;
using System.Linq;

namespace Dbosoft.YaNco.TypeMapping;

public class DefaultConverterResolver : IRfcConverterResolver
{
    private readonly IEnumerable<Type> _toRfcConverters;
    private readonly IEnumerable<Type> _fromRfcConverters;

    public static DefaultConverterResolver CreateWithBuildInConverters(
        IEnumerable<Type> fromRfcConverters = null, IEnumerable<Type> toRfcConverters = null)
    {
        fromRfcConverters = (fromRfcConverters ?? Type.EmptyTypes).Append(new []
        {
            typeof(DateTimeValueConverter),
            typeof(ByteValueConverter),
            typeof(DictionaryFromAbapStructureValueConverter),
            typeof(ListFromAbapTableValueConverter),
            typeof(DefaultFromAbapValueConverter<>),
        });

        toRfcConverters = (toRfcConverters ?? Type.EmptyTypes).Append(new[]
        {
            typeof(IntValueConverter<>),
            typeof(LongValueConverter<>),
            typeof(StringValueConverter<>),
            typeof(ByteValueConverter),
            typeof(DateTimeValueConverter)
        });
            
        return new DefaultConverterResolver(
            fromRfcConverters, toRfcConverters);
    }

    public DefaultConverterResolver(IEnumerable<Type> fromRfcConverters, IEnumerable<Type> toRfcConverters)
    {
        _fromRfcConverters = fromRfcConverters;
        _toRfcConverters = toRfcConverters;
    }

    protected virtual object CreateConverter(Type type, Type targetType = null, Type abapType = null)
    {
        if (!type.IsGenericTypeDefinition)
            return Activator.CreateInstance(type);

        var typeArguments = new List<Type>();
        foreach (var argument in type.GetGenericArguments())
        {
            if (targetType is not null && !argument.IsSubclassOf(typeof(AbapValue)))
            {
                typeArguments.Add(targetType);
            }
            else
            {
                if (abapType == null) continue;

                if (abapType.IsSubclassOf(typeof(AbapValue)))
                    typeArguments.Add(abapType);
            }
        }

        return Activator.CreateInstance(type.MakeGenericType(typeArguments.ToArray()));
    }

    public IEnumerable<IToAbapValueConverter<T>> GetToRfcConverters<T>(RfcType rfcType)
    {
        return _toRfcConverters
            .Map(type => CreateConverter(type, typeof(T)) as IToAbapValueConverter<T>)
            .Where(c => c != null)
            .Where(c => c.CanConvertFrom(rfcType));

    }

    public IEnumerable<IFromAbapValueConverter<T>> GetFromRfcConverters<T>(RfcType rfcType, Type abapValueType)
    {
        return _fromRfcConverters
            .Map(type =>
                (CreateConverter(type, typeof(T), abapValueType)) as
                IFromAbapValueConverter<T>)
            .Where(c => c != null)
            .Where(c => c.CanConvertTo(rfcType));
    }


}