using System.Collections.Generic;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IStructure : IDataContainer
    {
        /// <summary>
        /// Get RfcFieldInfo for all fields in structure
        /// </summary>
        /// <returns></returns>
        Either<RfcErrorInfo, RfcFieldInfo[]> GetFieldInfos();

        /// <summary>
        /// Convert all fields in structure to a dictionary. FieldName will be key in dictionary. Each field value will be
        /// a unconverted AbapValue. Only flat structures are supported.
        /// </summary>
        /// <returns></returns>
        Either<RfcErrorInfo, IDictionary<string,AbapValue>> ToDictionary();

        /// <summary>
        /// Sets the structure fields from a dictionary. The key of the dictionary has to be the field name.
        /// The field will not be filled if the key not exists in dictionary.
        /// Only flat structures are supported.
        /// </summary>
        /// <typeparam name="T">Dictionary value type.</typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        Either<RfcErrorInfo, Unit> SetFromDictionary<T>(IDictionary<string, T> dictionary);
    }
}