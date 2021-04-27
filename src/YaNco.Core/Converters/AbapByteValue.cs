using System;
using System.Globalization;

namespace Dbosoft.YaNco.Converters
{
    public class AbapByteValue : AbapValue
    {
        public readonly byte[] Value;

        public AbapByteValue(RfcFieldInfo fieldInfo, byte[] value) :
            base(fieldInfo)
        {
            Value = value;
        }
    }
}