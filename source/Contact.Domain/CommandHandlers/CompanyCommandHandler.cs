﻿using System.Threading.Tasks;
using Contact.Domain.Aggregates;
using Contact.Domain.Commands;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.CommandHandlers
{
    public class CompanyCommandHandler :
        Handles<AddCompanyAdmin>,
        Handles<RemoveCompanyAdmin>,
        Handles<AddEmployee>,
        Handles<TerminateEmployee>,
        Handles<AddBusyTime>,
        Handles<RemoveBusyTime>,
        Handles<ConfirmBusyTimeEntries>,
        Handles<UpdateBusyTimeSetEndDate>,
        Handles<UpdateBusyTimeChangePercentage>
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
            var admin = await _employeeRepository.GetByIdAsync(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeAdmin = await _employeeRepository.GetByIdAsync(message.NewAdminId);
            if (employeeToBeAdmin == null) throw new UnknownItemException("Unknown ID for employee to be admin");


            company.AddCompanyAdmin(employeeToBeAdmin, message.CreatedBy, message.CorrelationId);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }

        public async Task Handle(RemoveCompanyAdmin message)
        {
            var admin = await _employeeRepository.GetByIdAsync(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            var employeeToBeRemoved = await _employeeRepository.GetByIdAsync(message.AdminId);
            if (employeeToBeRemoved == null) throw new UnknownItemException("Unknown ID for admin to be removed");

            company.RemoveCompanyAdmin(employeeToBeRemoved, message.CreatedBy, message.CorrelationId);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }


        public async Task Handle(AddEmployee message)
        {
            var admin = await _employeeRepository.GetByIdAsync(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

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
                message.GlobalId, message.LoginId, message.FirstName, message.MiddleName, message.LastName, message.DateOfBirth,
                message.JobTitle, message.OfficeName, message.PhoneNumber, message.Email, message.HomeAddress, message.Photo, new Person(admin.Id, admin.Name), message.CorrelationId);

            company.AddNewEmployeeToCompany(newEmployee, new Person(admin.Id, admin.Name), message.CorrelationId);

            await _employeeRepository.SaveAsync(newEmployee, Constants.NewVersion);
            await _companyRepository.SaveAsync(company, message.BasedOnVersion);
        }

        public async Task Handle(TerminateEmployee message)
        {
            var admin = await _employeeRepository.GetByIdAsync(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

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
            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");



            if (!company.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.AddBusyTime(company.Id, company.Name, message.Start, message.End,
                message.PercentageOccpied, message.Comment, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(RemoveBusyTime message)
        {
            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.RemoveBusyTime(company.Id, company.Name, message.BusyTimeId,
                message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(ConfirmBusyTimeEntries message)
        {
            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            if (!company.HasUser(message.EmployeeId)) throw new UnknownItemException("User not in office");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.ConfirmBusyTimeEntries(company.Id, company.Name, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(UpdateBusyTimeSetEndDate message)
        {
            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.SetBusyTimeEnd(company.Id, company.Name, message.BusyTimeId, message.NewEnd, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }

        public async Task Handle(UpdateBusyTimeChangePercentage message)
        {
            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");

            var employee = await _employeeRepository.GetByIdAsync(message.EmployeeId);
            if (employee == null) throw new UnknownItemException("Unknown ID for employee");

            employee.SetPercentageOccupied(company.Id, company.Name, message.BusyTimeId, message.NewpercentageOccpied, message.CreatedBy, message.CorrelationId);

            await _employeeRepository.SaveAsync(employee, message.BasedOnVersion);
        }
    }
}
