using System.Collections.Generic;

namespace Dbosoft.YaNco.TypeMapping
{
    public class AbapTableValues : AbapValue
    {
        public readonly IEnumerable<IDictionary<string, AbapValue>> Values;

        public AbapTableValues(RfcFieldInfo fieldInfo, IEnumerable<IDictionary<string, AbapValue>> values) :
            base(fieldInfo)
        {
            Values = values;
        }
    }
}