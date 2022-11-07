using System;
using System.Collections.Generic;

namespace Dbosoft.YaNco.TypeMapping
{
    public class AbapStructureValue : AbapValue
    {
        public readonly IDictionary<string, AbapValue> Values;

        public AbapStructureValue(RfcFieldInfo fieldInfo, IDictionary<string, AbapValue> values) :
            base(fieldInfo)
        {
            Values = values;
        }
    }
}