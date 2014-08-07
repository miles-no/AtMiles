namespace Contact.Domain.Commands
{
    public class TerminateEmployee : Command
    {
        public string CompanyId { get; private set; }
        public string EmployeeId { get; private set; }

        public TerminateEmployee(string companyId, string employeeId)
        {
            CompanyId = companyId;
            EmployeeId = employeeId;
        }
    }
}
