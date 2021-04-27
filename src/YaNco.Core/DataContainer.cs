using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.YaNco
{
    internal abstract class DataContainer : IDataContainer
    {
        private readonly IDataContainerHandle _handle;
        private readonly IRfcRuntime _rfcRuntime;

        protected DataContainer(IDataContainerHandle handle, IRfcRuntime rfcRuntime)
        {
            _handle = handle;
            _rfcRuntime = rfcRuntime;
        }

        public Either<RfcErrorInfo, Unit> SetField<T>(string name, T value)
        {
            return _rfcRuntime.SetFieldValue<T>(_handle, value, () => GetFieldInfo(name));
        }

        protected abstract Either<RfcErrorInfo, RfcFieldInfo> GetFieldInfo(string name);

        public Either<RfcErrorInfo, T> GetField<T>(string name)
        {
            return _rfcRuntime.GetFieldValue<T>(_handle,() => GetFieldInfo(name));
        }

        public Either<RfcErrorInfo, Unit> SetFieldBytes(string name, byte[] buffer, long bufferLength)
        {
            return _rfcRuntime.SetBytes(_handle, name, buffer, bufferLength);
        }

        public Either<RfcErrorInfo, byte[]> GetFieldBytes(string name)
        {
            return _rfcRuntime.GetBytes(_handle, name);
        }


        public Either<RfcErrorInfo, IStructure> GetStructure(string name)
        {
            return _rfcRuntime.GetStructure(_handle, name).Map(handle => (IStructure) new Structure(handle, _rfcRuntime));
        }

        public Either<RfcErrorInfo, ITable> GetTable(string name)
        {
            return _rfcRuntime.GetTable(_handle, name).Map(handle => (ITable) new Table(handle, _rfcRuntime));
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