using System.Collections.Generic;

namespace Dbosoft.YaNco.TypeMapping
{
    public class AbapTableValue : AbapValue
    {
        public readonly IEnumerable<IDictionary<string, AbapValue>> Values;

        public AbapTableValue(RfcFieldInfo fieldInfo, IEnumerable<IDictionary<string, AbapValue>> values) :
            base(fieldInfo)
        {
            Values = values;
        }
    }
}