using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping
{
    public interface IFieldMapper
    {
        Either<RfcErrorInfo, Unit> SetField<T>(T value, FieldMappingContext context);
        Either<RfcErrorInfo, T> GetField<T>(FieldMappingContext context);
        
    }
}