namespace Contact.Domain.Commands
{
    public class TerminateEmployee : Command
    {
        public string EmployeeId { get; private set; }

        public TerminateEmployee(string employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
