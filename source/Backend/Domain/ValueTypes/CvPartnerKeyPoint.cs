namespace no.miles.at.Backend.Domain.ValueTypes
{
    public class CvPartnerKeyPoint
    {
        public readonly string InternationalName;
        public readonly string LocalName;
        public readonly string InternationalDescription;
        public readonly string LocalDescription;

        public CvPartnerKeyPoint(string internationalName, string localName, string internationalDescription, string localDescription)
        {
            InternationalName = internationalName;
            LocalName = localName;
            InternationalDescription = internationalDescription;
            LocalDescription = localDescription;
        }
    }
}
