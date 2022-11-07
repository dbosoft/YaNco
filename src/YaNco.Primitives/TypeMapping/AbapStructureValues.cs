using System;
using System.Collections.Generic;

namespace Dbosoft.YaNco.TypeMapping
{
    public class AbapStructureValues : AbapValue
    {
        public readonly IDictionary<string, AbapValue> Values;

        public AbapStructureValues(RfcFieldInfo fieldInfo, IDictionary<string, AbapValue> values) :
            base(fieldInfo)
        {
            Values = values;
        }
    }
}