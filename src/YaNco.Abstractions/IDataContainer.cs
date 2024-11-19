using System;
using JetBrains.Annotations;
using LanguageExt;

namespace Dbosoft.YaNco;

[PublicAPI]
public interface IDataContainer : IDisposable
{
    Either<RfcError, Unit> SetField<T>(string name, T value);
    Either<RfcError, T> GetField<T>(string name);
    Either<RfcError, Unit> SetFieldBytes(string name, byte[] buffer, long bufferLength);
    Either<RfcError, byte[]> GetFieldBytes(string name);
    Either<RfcError, IStructure> GetStructure(string name);
    Either<RfcError, ITable> GetTable(string name);
    Either<RfcError, ITypeDescriptionHandle> GetTypeDescription();

    /// <summary>
    /// Direct access to the container handle.
    /// </summary>
    /// <remarks>
    /// Use this property only if you would like to call runtime api methods that are not covered by the <see cref="IDataContainer"/> interface.
    /// </remarks>
    IDataContainerHandle Handle { get; }
}