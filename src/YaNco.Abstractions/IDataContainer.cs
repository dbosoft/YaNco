using System;
using LanguageExt;

namespace Dbosoft.SAP.NWRfc
{
    public interface IDataContainer : IDisposable
    {
        Either<RfcErrorInfo, Unit> SetField<T>(string name, T value);
        Either<RfcErrorInfo, T> GetField<T>(string name);
        Either<RfcErrorInfo, Unit> SetFieldBytes(string name, byte[] buffer, long bufferLength);
        Either<RfcErrorInfo, byte[]> GetFieldBytes(string name);
        Either<RfcErrorInfo, IStructure> GetStructure(string name);
        Either<RfcErrorInfo, ITable> GetTable(string name);
    }
}