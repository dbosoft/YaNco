using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class DefaultFromAbapValueConverter<T> : IFromAbapValueConverter<T>
    {
        public T ConvertTo(AbapValue abapValue)
        {
            return (T) Convert.ChangeType(abapValue, typeof(T), CultureInfo.InvariantCulture);
        }

        public bool CanConvertTo(AbapValue abapValue)
        {
            try
            {
                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Convert.ChangeType(abapValue, typeof(T), CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}