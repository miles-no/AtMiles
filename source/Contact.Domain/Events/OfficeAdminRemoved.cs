namespace Contact.Domain.Events
{
    public class OfficeAdminRemoved : Event
    {
        public string AdminId { get; private set; }
        public string AdminName { get; private set; }

        public string OfficeId { get; private set; }
        public string OfficeName { get; private set; }

        public OfficeAdminRemoved(string adminId, string adminName, string officeId, string officeName)
        {
            AdminId = adminId;
            AdminName = adminName;
            OfficeId = officeId;
            OfficeName = officeName;
        }
    }
}
