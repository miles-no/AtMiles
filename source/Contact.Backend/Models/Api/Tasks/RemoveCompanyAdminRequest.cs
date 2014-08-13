namespace Contact.Backend.Models.Api.Tasks
{
    public class RemoveCompanyAdminRequest
    {
        public string CompanyId { get; set; }
        public string AdminId { get; set; }
    }
}