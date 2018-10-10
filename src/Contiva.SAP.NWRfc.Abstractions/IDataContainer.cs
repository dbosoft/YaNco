using System;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public interface IDataContainer
    {
        Either<RfcErrorInfo, Unit> SetField(string name, string value);
        Either<RfcErrorInfo, T> GetField<T>(string name);
        Either<RfcErrorInfo, IStructure> GetStructure(string name);
        Either<RfcErrorInfo, ITable> GetTable(string name);
    }
}