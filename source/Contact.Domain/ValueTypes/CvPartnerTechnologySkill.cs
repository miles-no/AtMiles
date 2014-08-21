namespace Contact.Domain.ValueTypes
{
    public class CvPartnerTechnologySkill
    {
        public readonly string InternationalName;
        public readonly string LocalName;

        public CvPartnerTechnologySkill(string internationalName, string localName)
        {
            InternationalName = internationalName;
            LocalName = localName;
        }
    }
}
