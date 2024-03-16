using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public interface IFieldMapper
    {
        Either<RfcError, Unit> SetField<T>(T value, FieldMappingContext context);
        Either<RfcError, T> GetField<T>(FieldMappingContext context);

        Either<RfcError, T> FromAbapValue<T>(AbapValue abapValue);
        Either<RfcError, AbapValue> ToAbapValue<T>(T value, RfcFieldInfo fieldInfo);

    }
}