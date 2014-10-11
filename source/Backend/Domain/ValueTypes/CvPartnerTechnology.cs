namespace no.miles.at.Backend.Domain.ValueTypes
{
    public class CvPartnerTechnology
    {
        public readonly string InternationalCategory;
        public readonly string LocalCategory;
        public readonly CvPartnerTechnologySkill[] Skills;

        public CvPartnerTechnology(string internationalCategory, string localCategory, CvPartnerTechnologySkill[] skills)
        {
            InternationalCategory = internationalCategory;
            LocalCategory = localCategory;
            Skills = skills;
        }
    }
}
