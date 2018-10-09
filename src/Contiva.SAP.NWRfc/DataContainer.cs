using System;
using LanguageExt;

namespace Contiva.SAP.NWRfc
{
    public class DataContainer
    {
        private readonly IDataContainerHandle _handle;
        private readonly IRfcRuntime _rfcRuntime;

        public DataContainer(IDataContainerHandle handle, IRfcRuntime rfcRuntime)
        {
            _handle = handle;
            _rfcRuntime = rfcRuntime;
        }

        public Either<RfcErrorInfo, Unit> SetField(string name, string value)
        {
            return _rfcRuntime.SetString(_handle, name, value);

        }

        public Either<RfcErrorInfo, T> GetField<T>(string name)
        {
            return _rfcRuntime.GetString(_handle, name).Map(r =>
            {
                object value = r;

                if (typeof(T) == typeof(bool))
                {
                    value = !string.IsNullOrWhiteSpace(value.ToString());
                }

                return (T) Convert.ChangeType(value, typeof(T));
            });
        }


        public Either<RfcErrorInfo, Structure> GetStructure(string name)
        {
            return _rfcRuntime.GetStructure(_handle, name).Map(handle => new Structure(handle, _rfcRuntime));
        }

        public Either<RfcErrorInfo, Table> GetTable(string name)
        {
            return _rfcRuntime.GetTable(_handle, name).Map(handle => new Table(handle, _rfcRuntime));
        }
    }
}