namespace Contact.Backend.Models.Api.Tasks
{
    public class MoveEmployeeToNewOfficeRequest
    {
        public string CompanyId { get; set; }
        public string OldOfficeId { get; set; }
        public string NewOfficeId { get; set; }
        public string EmployeeId { get; set; }
    }
}