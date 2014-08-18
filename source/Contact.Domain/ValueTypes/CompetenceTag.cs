namespace Contact.Domain.ValueTypes
{
    public class CompetenceTag
    {
        public readonly string LocalCategory;
        public readonly string InternationalCategory;
        public readonly string LocalSubject;
        public readonly string InternationalSubject;

        public CompetenceTag(string localCategory, string internationalCategory, string localSubject, string internationalSubject)
        {
            LocalCategory = localCategory;
            LocalSubject = localSubject;
            InternationalCategory = internationalCategory;
            InternationalSubject = internationalSubject;
        }
    }
}
