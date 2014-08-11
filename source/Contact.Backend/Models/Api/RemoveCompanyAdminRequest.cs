namespace Contact.Backend.Models.Api
{
    public class RemoveCompanyAdminRequest
    {
        public string CompanyId { get; set; }
        public string AdminId { get; set; }
    }
}