using System;
using Dbosoft.YaNco.TypeMapping;
using LanguageExt;

namespace Dbosoft.YaNco.Traits;

// ReSharper disable once InconsistentNaming
public interface SAPRfcFieldIO
{
    Either<RfcError, Unit> SetString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetString(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetDateString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetDateString(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetTimeString(IDataContainerHandle containerHandle, string name,
        string value);

    Either<RfcError, string> GetTimeString(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetInt(IDataContainerHandle containerHandle, string name, int value);
    Either<RfcError, int> GetInt(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetLong(IDataContainerHandle containerHandle, string name, long value);
    Either<RfcError, long> GetLong(IDataContainerHandle containerHandle, string name);
    Either<RfcError, Unit> SetBytes(IDataContainerHandle containerHandle, string name, byte[] buffer, long bufferLength);
    Either<RfcError, byte[]> GetBytes(IDataContainerHandle containerHandle, string name);

    Either<RfcError, Unit> SetFieldValue<T>(IDataContainerHandle handle, T value, Func<Either<RfcError, RfcFieldInfo>> func);
    Either<RfcError, T> GetFieldValue<T>(IDataContainerHandle handle, Func<Either<RfcError, RfcFieldInfo>> func);

    Either<RfcError, T> GetValue<T>(AbapValue abapValue);
    Either<RfcError, AbapValue> SetValue<T>(T value, RfcFieldInfo fieldInfo);

}