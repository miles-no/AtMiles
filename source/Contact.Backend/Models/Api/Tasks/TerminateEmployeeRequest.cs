namespace Contact.Backend.Models.Api.Tasks
{
    public class TerminateEmployeeRequest
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string EmployeeId { get; set; }
    }
}