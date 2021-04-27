using LanguageExt;

namespace Dbosoft.YaNco.Converters
{
    public interface IFieldMapper
    {
        Either<RfcErrorInfo, Unit> SetField<T>(T value, FieldMappingContext context);
        Either<RfcErrorInfo, T> GetField<T>(FieldMappingContext context);
        
    }
}