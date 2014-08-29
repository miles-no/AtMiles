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
        Handles<TerminateEmployee>,
        Handles<MoveEmployeeToNewOffice>,
        Handles<AddBusyTime>,
        Handles<RemoveBusyTime>,
        Handles<ConfirmBusyTimeEntries>
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
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeAdmin = _employeeRepository.GetById(message.NewAdminId);
            if (employeeToBeAdmin == null) throw new UnknownItemException("Unknown ID for employee to be admin");


            company.AddCompanyAdmin(employeeToBeAdmin, message.CreatedBy, message.CorrelationId);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (employeeToBeRemoved == null) throw new UnknownItemException("Unknown ID for admin to be removed");

            company.RemoveCompanyAdmin(employeeToBeRemoved, message.CreatedBy, message.CorrelationId);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var adminToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (adminToBeRemoved == null) throw new UnknownItemException("Unknown ID for admin to be removed");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            company.RemoveOfficeAdmin(message.OfficeId, adminToBeRemoved, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddOfficeAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var newAdmin = _employeeRepository.GetById(message.AdminId);
            if (newAdmin == null) throw new UnknownItemException("Unknown ID for admin to be added");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            company.AddOfficeAdmin(message.OfficeId, newAdmin, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(OpenOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            if (company.IsOffice(message.OfficeId)) throw new AlreadyExistingItemException("Office ID already existing");

            company.OpenOffice(message.OfficeId, message.OfficeName, message.Address, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(CloseOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for office");

            company.CloseOffice(message.OfficeId, message.CreatedBy, message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddEmployee message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = company.GetOffice(message.OfficeId);

            Employee existingUser = null;
            try
            {
                existingUser = _employeeRepository.GetById(message.GlobalId);
            }
            catch (UnknownItemException) { }

            if (existingUser != null) throw new AlreadyExistingItemException("User with same ID already exists");

            if (!company.HasOfficeAdminAccess(admin.Id, office.Id)) throw new NoAccessException("No access to complete this operation");

            var newEmployee = new Employee();
            newEmployee.CreateNew(company.Id, company.Name, office.Id, office.Name,
                message.GlobalId, message.LoginId, message.FirstName, message.MiddleName, message.LastName, message.DateOfBirth,
                message.JobTitle, message.PhoneNumber, message.Email, message.HomeAddress, message.Photo, new Person(admin.Id, admin.Name), message.CorrelationId);

            company.AddNewEmployeeToOffice(office.Id, newEmployee, new Person(admin.Id, admin.Name), message.CorrelationId);

            _employeeRepository.Save(newEmployee, Constants.NewVersion);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(TerminateEmployee message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = company.GetOffice(message.OfficeId);

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            CheckIfHandlingSelf(admin, employee);

            if (!company.HasOfficeAdminAccess(admin.Id, office.Id)) throw new NoAccessException("No access to complete this operation");

            company.RemoveEmployee(office.Id, employee, new Person(admin.Id, admin.Name), message.CorrelationId);
            employee.Terminate(company.Id, company.Name, office.Id, office.Name, new Person(admin.Id, admin.Name), message.CorrelationId);

            _employeeRepository.Save(employee, Constants.IgnoreVersion);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        private static void CheckIfHandlingSelf(Employee admin, Employee employee)
        {
            if (admin.Id == employee.Id) throw new NoAccessException("Cannot perform operation on self");
        }

        public void Handle(MoveEmployeeToNewOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            if (!company.IsOffice(message.OldOfficeId)) throw new UnknownItemException("Unknown ID for Office to be moved from");

            if (!company.IsOffice(message.NewOfficeId)) throw new UnknownItemException("Unknown ID for Office to move to.");

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            company.MoveEmployeeToNewOffice(employee, message.OldOfficeId, message.NewOfficeId,
                new Person(admin.Id, admin.Name), message.CorrelationId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddBusyTime message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = company.GetOffice(message.OfficeId);

            if (!office.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.AddBusyTime(company.Id, company.Name, office.Id, office.Name, message.Start, message.End,
                message.PercentageOccpied, message.Comment, message.CreatedBy, message.CorrelationId);

            _employeeRepository.Save(employee, message.BasedOnVersion);
        }

        public void Handle(RemoveBusyTime message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = company.GetOffice(message.OfficeId);

            if (!office.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.RemoveBusyTime(company.Id, company.Name, office.Id, office.Name, message.BusyTimeId,
                message.CreatedBy, message.CorrelationId);

            _employeeRepository.Save(employee, message.BasedOnVersion);
        }

        public void Handle(ConfirmBusyTimeEntries message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.IsOffice(message.OfficeId)) throw new UnknownItemException("Unknown ID for Office");

            var office = company.GetOffice(message.OfficeId);

            if (!office.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = _employeeRepository.GetById(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.ConfirmBusyTimeEntries(company.Id, company.Name, office.Id, office.Name, message.CreatedBy, message.CorrelationId);

            _employeeRepository.Save(employee, message.BasedOnVersion);

        }
    }
}
