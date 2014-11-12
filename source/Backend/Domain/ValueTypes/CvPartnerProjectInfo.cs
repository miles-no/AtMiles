namespace no.miles.at.Backend.Domain.ValueTypes
{
    public class CvPartnerProjectInfo
    {
        public string Customer;
        public string CustomerDescription;
        public string CustomerValueProposition;
        public string Description;
        public bool? Disabled;
        public string[] ExcludeTags;
        public string ExpectedRollOffDate;
        public string Industry;
        public string IntRelatedWorkExperienceId;
        public object LongDescription;
        public object MonthFrom;
        public object MonthTo;
        public int? Order;
        public Role[] Roles;
        public bool? Starred;
        public string[] Tags;
        public object YearFrom;
        public object YearTo;

        public class Role
        {
            public string Name;
            public string LongDescription;
            public bool Disabled;
            public int? Order;
            public bool Starred;
        }
    }
}
