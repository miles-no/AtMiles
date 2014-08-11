namespace Contact.Backend.Models.Api
{
    public class TerminateEmployeeRequest
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string EmployeeId { get; set; }
    }
}