namespace Dbosoft.SAP.NWRfc
{
    public class RfcParameterInfo : RfcFieldInfo
    {
        public readonly RfcDirection Direction;

        public readonly string DefaultValue;

        public readonly string ParameterText;

        public readonly bool Optional;

        public RfcParameterInfo(string name, RfcType type, RfcDirection direction, uint nucLength, uint ucLength, uint decimals, string defaultValue, string parameterText, bool optional)
            :base(name, type, nucLength, ucLength, decimals)
        {
            Direction = direction;
            DefaultValue = defaultValue;
            ParameterText = parameterText;
            Optional = optional;
        }
    }

    public class RfcFieldInfo
    {
        public readonly string Name;

        public readonly RfcType Type;

        public readonly uint NucLength;

        public readonly uint UcLength;

        public readonly uint Decimals;


        public RfcFieldInfo(string name, RfcType type, uint nucLength, uint ucLength, uint decimals)
        {
            Name = name;
            Type = type;
            NucLength = nucLength;
            UcLength = ucLength;
            Decimals = decimals;
        }
    }

}