namespace Contact.Backend.Models.Api.Tasks
{
    public class RemoveBusyTimeRequest
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string BustTimeEntryId { get; set; }
    }
}