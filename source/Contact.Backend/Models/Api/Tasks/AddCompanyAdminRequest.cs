namespace Contact.Backend.Models.Api.Tasks
{
    public class AddCompanyAdminRequest
    {
        public string CompanyId { get; set; }
        public string NewAdminId { get; set; }
    }
}