using Contact.Domain.Aggregates;
using Contact.Domain.Commands;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.CommandHandlers
{
    public class CompanyCommandHandler :
        Handles<AddCompanyAdmin>,
        Handles<RemoveCompanyAdmin>,
        Handles<RemoveOfficeAdmin>,
        Handles<AddOfficeAdmin>,
        Handles<OpenOffice>,
        Handles<CloseOffice>,
        Handles<AddEmployee>,
        Handles<TerminateEmployee>
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public CompanyCommandHandler(IRepository<Company> companyRepository, IRepository<Employee> employeeRepository)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

        public void Handle(AddCompanyAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException();

            var employeeToBeAdmin = _employeeRepository.GetById(message.NewAdminId);
            if(employeeToBeAdmin == null) throw new UnknownItemException();


            company.AddCompanyAdmin(employeeToBeAdmin, message.CreatedBy, message.CorrelationId);
            _companyRepository.Save(company,message.BasedOnVersion);
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException();

            var employeeToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (employeeToBeRemoved == null) throw new UnknownItemException();

            company.RemoveCompanyAdmin(employeeToBeRemoved, message.CreatedBy, message.CorrelationId);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var adminToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (adminToBeRemoved == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            company.RemoveOfficeAdmin(message.OfficeId, adminToBeRemoved, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddOfficeAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var newAdmin = _employeeRepository.GetById(message.AdminId);
            if (newAdmin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);
            
            company.AddOfficeAdmin(message.OfficeId, newAdmin, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(OpenOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            if(!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException();

            if (company.IsOffice(message.OfficeId)) throw new AlreadyExistingItemException();

            company.OpenOffice(message.OfficeId, message.OfficeName, message.Address, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(CloseOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException();

            if(!company.IsOffice(message.OfficeId)) throw new UnknownItemException();

            company.CloseOffice(message.OfficeId, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddEmployee message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            if(!company.IsOffice(message.OfficeId)) throw new UnknownItemException();

            var office = company.GetOffice(message.OfficeId);

            Employee existingUser = null;
            try
            {
                existingUser = _employeeRepository.GetById(message.GlobalId);
            }
            catch (UnknownItemException){}

            if(existingUser != null) throw new AlreadyExistingItemException();

            if(!company.HasAccessToAddEmployeeToOffice(admin.Id, office.Id)) throw new NoAccessException();

            var newEmployee = new Employee();
            newEmployee.CreateNew(company.Id, company.Name, office.Id, office.Name,
                message.GlobalId, message.FirstName, message.MiddleName, message.LastName, message.DateOfBirth,
                message.JobTitle, message.PhoneNumber, message.Email, message.HomeAddress, message.Photo, new Person(admin.Id, admin.Name), message.CorrelationId);

            company.AddNewEmployeeToOffice(office.Id, newEmployee, new Person(admin.Id, admin.Name), message.CorrelationId);

            _employeeRepository.Save(newEmployee, Constants.NewVersion);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(TerminateEmployee message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException();
            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException();

            var office = company.GetOffice(message.OfficeId);

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if(employee == null) throw new UnknownItemException();

            CheckIfHandlingSelf(admin, employee);

            if (!company.HasAccessToAddEmployeeToOffice(admin.Id, office.Id)) throw new NoAccessException();

            company.RemoveEmployee(office.Id, employee, new Person(admin.Id, admin.Name), message.CorrelationId);
            employee.Terminate(company.Id, company.Name, office.Id, office.Name, new Person(admin.Id, admin.Name), message.CorrelationId);

            _employeeRepository.Save(employee, Constants.IgnoreVersion);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        private static void CheckIfHandlingSelf(Employee admin, Employee employee)
        {
            if (admin.Id == employee.Id) throw new NoAccessException();
        }
    }
}
