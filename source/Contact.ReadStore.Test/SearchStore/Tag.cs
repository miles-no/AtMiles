namespace Contact.ReadStore.Test.SearchStore
{
    public class Tag
    {
        private string category = string.Empty;
        private string competency;
        private string internationalCompentency;
        private string internationalCategory;

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        public string Competency
        {
            get { return competency; }
            set { competency =   value; }
        }

        public string InternationalCompentency
        {
            get { return internationalCompentency; }
            set { internationalCompentency = value; }
        }

        public string InternationalCategory
        {
            get { return internationalCategory; }
            set { internationalCategory = value; }
        }
    }
}