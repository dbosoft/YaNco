using System;
using Dbosoft.YaNco;
using LanguageExt;

namespace Dbosoft.YaNco
{
    public interface IDataContainer : IDisposable
    {
        Either<RfcError, Unit> SetField<T>(string name, T value);
        Either<RfcError, T> GetField<T>(string name);
        Either<RfcError, Unit> SetFieldBytes(string name, byte[] buffer, long bufferLength);
        Either<RfcError, byte[]> GetFieldBytes(string name);
        Either<RfcError, IStructure> GetStructure(string name);
        Either<RfcError, ITable> GetTable(string name);
        Either<RfcError, ITypeDescriptionHandle> GetTypeDescription();
    }
}