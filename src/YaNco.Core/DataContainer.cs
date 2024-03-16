using System;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal abstract class DataContainer : IDataContainer
    {
        private readonly IDataContainerHandle _handle;
        protected readonly SAPRfcDataIO IO;

        protected DataContainer(IDataContainerHandle handle, SAPRfcDataIO io)
        {
            _handle = handle;
            IO = io;
        }

        public Either<RfcError, Unit> SetField<T>(string name, T value)
        {
            return IO.SetFieldValue(_handle, value, () => GetFieldInfo(name));
        }

        protected abstract Either<RfcError, RfcFieldInfo> GetFieldInfo(string name);

        public Either<RfcError, T> GetField<T>(string name)
        {
            return IO.GetFieldValue<T>(_handle,() => GetFieldInfo(name));
        }

        public Either<RfcError, Unit> SetFieldBytes(string name, byte[] buffer, long bufferLength)
        {
            return IO.SetBytes(_handle, name, buffer, bufferLength);
        }

        public Either<RfcError, byte[]> GetFieldBytes(string name)
        {
            return IO.GetBytes(_handle, name);
        }


        public Either<RfcError, IStructure> GetStructure(string name)
        {
            return IO.GetStructure(_handle, name).Map(handle => (IStructure) new Structure(handle, IO));
        }

        public Either<RfcError, ITable> GetTable(string name)
        {
            return IO.GetTable(_handle, name).Map(handle => (ITable) new Table(handle, IO));
        }

        public Either<RfcError, ITypeDescriptionHandle> GetTypeDescription()
        {
            return IO.GetTypeDescription(_handle);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _handle?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}