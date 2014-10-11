using System.Threading.Tasks;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.CommandHandlers
{
    public class CompanyCommandHandler :
        Handles<AddCompanyAdmin>,
        Handles<RemoveCompanyAdmin>,
        Handles<AddEmployee>,
        Handles<TerminateEmployee>,
        Handles<AddBusyTime>,
        Handles<RemoveBusyTime>,
        Handles<ConfirmBusyTimeEntries>,
        Handles<UpdateBusyTime>,
        Handles<SetDateOfBirth>,
        Handles<SetPrivateAddress>
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Employee> _employeeRepository;

        public CompanyCommandHandler(IRepository<Company> companyRepository, IRepository<Employee> employeeRepository)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task Handle(AddCompanyAdmin message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var admin = await GetAdminById(message.CreatedBy.Identifier);
            
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeAdmin = await GetEmployeeById(message.NewAdminId);

            company.AddCompanyAdmin(employeeToBeAdmin, message.CreatedBy, message.CorrelationId);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }

        public async Task Handle(RemoveCompanyAdmin message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var admin = await GetAdminById(message.CreatedBy.Identifier);
            
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeRemoved = await GetEmployeeById(message.AdminId);

            company.RemoveCompanyAdmin(employeeToBeRemoved, message.CreatedBy, message.CorrelationId);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }


        public async Task Handle(AddEmployee message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var admin = await GetAdminById(message.CreatedBy.Identifier);

            Employee existingUser = null;
            try
            {
                existingUser = await _employeeRepository.GetByIdAsync(message.GlobalId);
            }
            catch (UnknownItemException) { }

            if (existingUser != null) throw new AlreadyExistingItemException("User with same ID already exists");

            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var newEmployee = new Employee();
            newEmployee.CreateNew(company.Id, company.Name,
                message.GlobalId, message.LoginId, message.FirstName, message.MiddleName, message.LastName,
                new Person(admin.Id, admin.Name), message.CorrelationId);

            company.AddNewEmployeeToCompany(newEmployee, new Person(admin.Id, admin.Name), message.CorrelationId);

            await _employeeRepository.SaveAsync(newEmployee, Constants.NewVersion);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }

        public async Task Handle(TerminateEmployee message)
        {
            var admin = await GetAdminById(message.CreatedBy.Identifier);
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            CheckIfHandlingSelf(admin, employee);

            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            company.RemoveEmployee(employee, new Person(admin.Id, admin.Name), message.CorrelationId);
            employee.Terminate(company.Id, company.Name, new Person(admin.Id, admin.Name), message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, Constants.IgnoreVersion);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }

        private static void CheckIfHandlingSelf(Employee admin, Employee employee)
        {
            if (admin.Id == employee.Id) throw new NoAccessException("Cannot perform operation on self");
        }

        public async Task Handle(AddBusyTime message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.AddBusyTime(company.Id, company.Name, message.Start, message.End,
                message.PercentageOccpied, message.Comment, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(RemoveBusyTime message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.RemoveBusyTime(company.Id, company.Name, message.BusyTimeId,
                message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(ConfirmBusyTimeEntries message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.ConfirmBusyTimeEntries(company.Id, company.Name, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(UpdateBusyTime message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.UpdateBusyTimeEnd(company.Id, company.Name, message.BusyTimeId, message.Start, message.End, message.PercentageOccpied, message.Comment, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(SetDateOfBirth message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.SetDateOfBirth(company.Id, company.Name, message.DateOfBirth, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(SetPrivateAddress message)
        {
            var company = await GetCompanyById(message.CompanyId);
            var employee = await GetEmployeeById(message.EmployeeId);

            employee.SetPrivateAddress(company.Id, company.Name, message.PrivateAddress, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        private async Task<Company> GetCompanyById(string companyId)
        {
            var company = await _companyRepository.GetByIdAsync(companyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            return company;
        }

        private async Task<Employee> GetAdminById(string employeeId)
        {
            return await GetEmployeeById(employeeId, "Unknown ID for admin");
        }

        private async Task<Employee> GetEmployeeById(string employeeId)
        {
            return await GetEmployeeById(employeeId, "Unknown ID for employee");
        }

        private async Task<Employee> GetEmployeeById(string employeeId, string exceptionMessage)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null) throw new UnknownItemException(exceptionMessage);
            return employee;
        }
    }
}
