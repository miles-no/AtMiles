using System.Collections.Generic;
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
        Handles<ImportDataFromCvPartner>
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IImportDataFromCvPartner _cvPartnerImporter;

        public CompanyCommandHandler(IRepository<Company> companyRepository, IRepository<Employee> employeeRepository, IImportDataFromCvPartner cvPartnerImporter)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _cvPartnerImporter = cvPartnerImporter;
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

            //TODO: Check access


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
                message.JobTitle, message.PhoneNumber, message.Email, message.HomeAddress, message.Photo, message.Competence, new Person(admin.Id, admin.Name), message.CorrelationId);

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

        public void Handle(ImportDataFromCvPartner message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = _companyRepository.GetById(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            List<CvPartnerImportData> importData = _cvPartnerImporter.GetImportData().Result;

            if (importData != null)
            {
                foreach (var cvPartnerImportData in importData)
                {
                    string userId = company.GetUserIdByLoginId(new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email, string.Empty));
                    var employee = _employeeRepository.GetById(userId);

                    if (employee == null)
                    {
                        employee = new Employee();

                        var office = company.GetOfficeByName(cvPartnerImportData.OfficeName);
                        if (office == null)
                        {
                            company.OpenOffice(cvPartnerImportData.OfficeName, cvPartnerImportData.OfficeName, null, message.CreatedBy, message.CorrelationId);
                            office = company.GetOffice(cvPartnerImportData.OfficeName);
                        }

                        employee.CreateNew(company.Id, company.Name,office.Id, office.Name,Domain.Services.IdService.CreateNewId(), new Login(Constants.GoogleIdProvider,cvPartnerImportData.Email,string.Empty), cvPartnerImportData.FirstName, cvPartnerImportData.MiddleName, cvPartnerImportData.LastName,cvPartnerImportData.DateOfBirth,cvPartnerImportData.Title, cvPartnerImportData.Phone,cvPartnerImportData.Email,null,cvPartnerImportData.Photo,null,message.CreatedBy, message.CorrelationId );

                        company.AddNewEmployeeToOffice(office.Id,employee,message.CreatedBy, message.CorrelationId);
                        _companyRepository.Save(company, Constants.IgnoreVersion);
                    }

                    employee.ImportData(cvPartnerImportData, message.CreatedBy, message.CorrelationId);

                    _employeeRepository.Save(employee, Constants.IgnoreVersion);
                }
            }
        }

        private static void CheckIfHandlingSelf(Employee admin, Employee employee)
        {
            if (admin.Id == employee.Id) throw new NoAccessException("Cannot perform operation on self");
        }
    }
}
