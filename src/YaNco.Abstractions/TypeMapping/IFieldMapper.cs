using LanguageExt;

namespace Dbosoft.YaNco.TypeMapping;

/// <summary>
/// The field mapper is responsible for mapping between .NET types and SAP RFC types
/// </summary>
public interface IFieldMapper
{
    /// <summary>
    /// Sets a field value in a RFC field. For internal use by <see cref="SAPRfcFieldIO"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="context">Mapping context build by the IO.</param>
    /// <returns></returns>
    Either<RfcError, Unit> SetField<T>(T value, FieldMappingContext context);

    /// <summary>
    /// Gets a field value from a RFC field. For internal use by <see cref="SAPRfcFieldIO"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="context"></param>
    /// <returns></returns>
    Either<RfcError, T> GetField<T>(FieldMappingContext context);

    /// <summary>
    /// Maps a SAP RFC value to a .NET type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abapValue"></param>
    /// <returns></returns>
    Either<RfcError, T> FromAbapValue<T>(AbapValue abapValue);

    /// <summary>
    /// Maps a .NET value to a SAP RFC value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="fieldInfo"></param>
    /// <returns></returns>
    Either<RfcError, AbapValue> ToAbapValue<T>(T value, RfcFieldInfo fieldInfo);

}