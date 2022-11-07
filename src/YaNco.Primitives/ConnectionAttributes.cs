using JetBrains.Annotations;

namespace Dbosoft.YaNco
{
    [PublicAPI]
    public class ConnectionAttributes
    {
        public readonly string Destination;

        public readonly string Host;

        public readonly string PartnerHost;

        public readonly string SystemNumber;

        public readonly string SystemId;

        public readonly string Client;

        public readonly string User;

        public readonly string Language;

        public readonly string Trace;

        public readonly string IsoLanguage;

        public readonly string Codepage;

        public readonly string PartnerCodepage;

        public readonly string RfcRole;

        public readonly string Type;

        public readonly string PartnerType;

        public readonly string SystemRelease;

        public readonly string PartnerSystemRelease;

        public readonly string PartnerKernelRelease;

        public readonly string CpicConversionId;

        public readonly string ProgramName;

        public readonly string PartnerBytesPerChar;

        public readonly string PartnerSystemCodepage;

        public readonly string PartnerIPv4;

        public readonly string PartnerIPv6;

        public ConnectionAttributes(string destination, string host, string partnerHost, string systemNumber, string systemId, string client, string user, string language, string trace, string isoLanguage, string codepage, string partnerCodepage, string rfcRole, string type, string partnerType, string systemRelease, string partnerSystemRelease, string partnerKernelRelease, string cpicConversionId, string programName, string partnerBytesPerChar, string partnerSystemCodepage, string partnerIPv4, string partnerIPv6)
        {
            Destination = destination;
            Host = host;
            PartnerHost = partnerHost;
            SystemNumber = systemNumber;
            SystemId = systemId;
            Client = client;
            User = user;
            Language = language;
            Trace = trace;
            IsoLanguage = isoLanguage;
            Codepage = codepage;
            PartnerCodepage = partnerCodepage;
            RfcRole = rfcRole;
            Type = type;
            PartnerType = partnerType;
            SystemRelease = systemRelease;
            PartnerSystemRelease = partnerSystemRelease;
            PartnerKernelRelease = partnerKernelRelease;
            CpicConversionId = cpicConversionId;
            ProgramName = programName;
            PartnerBytesPerChar = partnerBytesPerChar;
            PartnerSystemCodepage = partnerSystemCodepage;
            PartnerIPv4 = partnerIPv4;
            PartnerIPv6 = partnerIPv6;
        }
    }
}