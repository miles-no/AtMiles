namespace Contact.Backend.Models.Api
{
    public class AddCompanyAdminRequest
    {
        public string CompanyId { get; set; }
        public string NewAdminId { get; set; }
    }
}