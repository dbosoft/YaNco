using System;
using System.Globalization;
using LanguageExt;

namespace Dbosoft.SAP.NWRfc
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
            switch (value)
            {
                case int intValue:
                    return _rfcRuntime.SetInt(_handle, name, intValue);
                case long longValue:
                    return _rfcRuntime.SetLong(_handle, name, longValue);
                case DateTime dateTime:
                    return GetFieldInfo(name)
                        .Bind(typeDesc =>
                        {
                            switch (typeDesc.Type)
                            {
                                case RfcType.DATE:
                                    return SetFieldAsDate(name, dateTime);
                                case RfcType.TIME:
                                    return SetFieldAsTime(name, dateTime);
                                default:
                                    return _rfcRuntime.SetString(_handle, name,
                                        (string)Convert.ChangeType(value, typeof(string)));
                            }
                        });

                default:
                {
                    return _rfcRuntime.SetString(_handle, name,
                        (string)Convert.ChangeType(value, typeof(string)));

                }
            }
        }

        protected abstract Either<RfcErrorInfo, RfcFieldInfo> GetFieldInfo(string name);

        public Either<RfcErrorInfo, T> GetField<T>(string name)
        {
            return GetFieldInfo(name)                
                .Bind(typeDesc =>
                {
                    switch (typeDesc.Type)
                    {
                        case RfcType.BYTE:
                            return GetFieldAsInt<T>(name);
                        case RfcType.NUM:
                            return GetFieldAsInt<T>(name);
                        case RfcType.INT:
                            return GetFieldAsInt<T>(name);
                        case RfcType.INT2:
                            return GetFieldAsInt<T>(name);
                        case RfcType.INT1:
                            return GetFieldAsInt<T>(name);
                        case RfcType.INT8:
                            return GetFieldAsLong<T>(name);
                        case RfcType.DATE:
                            return GetFieldAsDate<T>(name);
                        case RfcType.TIME:
                            return GetFieldAsTime<T>(name);
                        default:
                            return GetFieldAsString<T>(name);
                    }
                });
        }

        private Either<RfcErrorInfo, T> GetFieldAsString<T>(string name)
        {
            return _rfcRuntime.GetString(_handle, name).Map(r =>
            {
                object value = r;

                if (typeof(T) == typeof(bool))
                {
                    value = !string.IsNullOrWhiteSpace(r);
                }

                return (T)Convert.ChangeType(value, typeof(T));
            });
        }

        private Either<RfcErrorInfo, Unit> SetFieldAsDate(string name, DateTime value)
        {
            var dateString = value.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            return _rfcRuntime.SetDateString(_handle, name, dateString);
        }

        private Either<RfcErrorInfo, Unit> SetFieldAsTime(string name, DateTime value)
        {
            var dateString = value.ToString("HHmmss", CultureInfo.InvariantCulture);
            return _rfcRuntime.SetDateString(_handle, name, dateString);
        }

        private Either<RfcErrorInfo, T> GetFieldAsInt<T>(string name)
        {
            return _rfcRuntime.GetInt(_handle, name).Map(r =>
            {
                object value = r;

                if (typeof(T) == typeof(bool))
                {
                    value = r != 0;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            });
        }

        private Either<RfcErrorInfo, T> GetFieldAsLong<T>(string name)
        {
            return _rfcRuntime.GetLong(_handle, name).Map(r =>
            {
                object value = r;

                if (typeof(T) == typeof(bool))
                {
                    value = r != 0;
                }

                return (T)Convert.ChangeType(value, typeof(T));
            });
        }

        private Either<RfcErrorInfo, T> GetFieldAsDate<T>(string name)
        {
            return _rfcRuntime.GetDateString(_handle, name).Map(r =>
            {
                object value;
                if (typeof(T) == typeof(string))
                    value = r;
                else
                    value = DateTime.ParseExact(r, "yyyyMMdd", CultureInfo.InvariantCulture);
                
                return (T)Convert.ChangeType(value, typeof(T));
            });
        }

        private Either<RfcErrorInfo, T> GetFieldAsTime<T>(string name)
        {
            return _rfcRuntime.GetDateString(_handle, name).Map(r =>
            {
                object value;
                if (typeof(T) == typeof(string))
                    value = r;
                else
                    value = DateTime.ParseExact(r, "HHmmss", CultureInfo.InvariantCulture);

                return (T)Convert.ChangeType(value, typeof(T));
            });
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