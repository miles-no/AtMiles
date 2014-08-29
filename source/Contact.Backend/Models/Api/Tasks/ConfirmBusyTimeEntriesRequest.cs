namespace Contact.Backend.Models.Api.Tasks
{
    public class ConfirmBusyTimeEntriesRequest
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
    }
}