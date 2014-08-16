using System.Linq;
using Contact.Domain;
using Contact.Domain.Events.Employee;

namespace Contact.TestApp.InMemoryReadModel
{
    public class EmployeeHandler :
        Handles<EmployeeCreated>,
        Handles<EmployeeTerminated>
    {
        private readonly InMemoryRepository _repository;

        public EmployeeHandler(InMemoryRepository repository)
        {
            _repository = repository;
        }

        public void Handle(EmployeeCreated message)
        {
            var employee = new Employee
            {
                Id = message.GlobalId,
                FirstName = message.FirstName,
                MiddleName = message.MiddleName,
                LastName = message.LastName,
                Company = new SimpleCompany {CompanyId = message.CompanyId, CompanyName = message.CompanyName},
                Office = new SimpleOffice {OfficeId = message.OfficeId, OfficeName = message.OfficeName},
                DateOfBirth = message.DateOfBirth,
                JobTitle = message.JobTitle,
                PhoneNumber = message.PhoneNumber,
                Email = message.Email,
                HomeAddress = message.HomeAddress,
                Photo = message.Photo
            };
            _repository.Employees.Add(employee);
        }

        public void Handle(EmployeeTerminated message)
        {
            _repository.Employees.RemoveAll(e => e.Id == message.Id && e.Company.CompanyId == message.CompanyId && e.Office.OfficeId == message.OfficeId);
        }
    }
}
