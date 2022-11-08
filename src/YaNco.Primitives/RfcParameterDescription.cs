using System;

namespace Dbosoft.YaNco
{
    public class RfcParameterDescription : RfcParameterInfo
    {
        public IntPtr TypeDescriptionHandle { get; set; }

        public RfcParameterDescription(string name, RfcType type, RfcDirection direction, uint nucLength, uint ucLength, uint decimals, bool optional, string defaultValue) : base(name, type, direction, nucLength, ucLength, decimals, defaultValue, null, optional)
        {
        }
    }
}