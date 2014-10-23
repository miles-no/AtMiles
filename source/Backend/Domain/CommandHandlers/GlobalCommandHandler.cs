using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.CommandHandlers
{
    public class GlobalCommandHandler :
        Handles<AddNewCompanyToSystem>,
        Handles<ImportDataFromCvPartner>
    {
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Global> _globalRepository;
        private readonly IImportDataFromCvPartner _cvPartnerImporter;

        public GlobalCommandHandler(IRepository<Company> companyRepository, IRepository<Employee> employeeRepository, IRepository<Global> globalRepository, IImportDataFromCvPartner cvPartnerImporter)
        {
            _companyRepository = companyRepository;
            _employeeRepository = employeeRepository;
            _globalRepository = globalRepository;
            _cvPartnerImporter = cvPartnerImporter;
        }

        public async Task Handle(AddNewCompanyToSystem message)
        {
            var global = await _globalRepository.GetByIdAsync(Global.GlobalId);
            if(global == null) global = new Global();

            if(global.HasCompany(message.CompanyId)) throw new AlreadyExistingItemException("CompanyId already in system");

            var system = new Employee();
            system.CreateNew(message.CompanyId, message.CompanyName,
                Constants.SystemUserId, new Login(Constants.SystemUserId, Constants.SystemUserId), string.Empty, string.Empty, Constants.SystemUserId,
                new Person(Constants.SystemUserId, Constants.SystemUserId), message.CorrelationId);

            await _employeeRepository.SaveAsync(system, Constants.NewVersion);


            var systemAsPerson = new Person(system.Id, system.Name);

            var company = new Company();
            company.CreateNewCompany(message.CompanyId, message.CompanyName, system.Id, system.Name, DateTime.UtcNow, systemAsPerson, message.CorrelationId);

            global.AddCompany(company, systemAsPerson, message.CorrelationId);

            await _globalRepository.SaveAsync(global, Constants.NewVersion);
            await _companyRepository.SaveAsync(company, Constants.NewVersion);

            if (message.InitialAdmins != null)
            {
                foreach (var adminInfo in message.InitialAdmins)
                {
                    var admin = new Employee();
                    var adminId = adminInfo.Id;
                    if (string.IsNullOrEmpty(adminId))
                    {
                        adminId = IdService.CreateNewId();
                    }

                    admin.CreateNew(message.CompanyId, message.CompanyName, adminId, adminInfo.LoginId, adminInfo.FirstName, adminInfo.MiddleName, adminInfo.LastName, message.CreatedBy, message.CorrelationId);
                    await _employeeRepository.SaveAsync(admin, Constants.NewVersion);
                    company.AddNewEmployeeToCompany(admin,message.CreatedBy, message.CorrelationId);
                    company.AddCompanyAdmin(admin, message.CreatedBy, message.CorrelationId);
                }
            }
            await _companyRepository.SaveAsync(company, Constants.IgnoreVersion);
        }

        public async Task Handle(ImportDataFromCvPartner message)
        {
            var admin = await _employeeRepository.GetByIdAsync(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException("Unknown ID for admin");

            var company = await _companyRepository.GetByIdAsync(message.CompanyId);
            if (company == null) throw new UnknownItemException("Unknown ID for company");
            if (!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException("No access to complete this operation");

            List<CvPartnerImportData> importData = await _cvPartnerImporter.GetImportData();

            if (importData != null)
            {
                await AddOrUpdateUsers(message, importData, company);
                await RemoveUsersNotInCvPartnerAnyMore(message, company, importData);
            }
        }

        private async Task AddOrUpdateUsers(ImportDataFromCvPartner message, IEnumerable<CvPartnerImportData> importData, Company company)
        {
            foreach ( var employee in importData)
            {
                await AddOrUpdateUser(message, company, employee);
            }
        }

        private async Task AddOrUpdateUser(ImportDataFromCvPartner message, Company company, CvPartnerImportData cvPartnerImportData)
        {
            var userId = company.GetUserIdByLoginId(new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email));
            var employee = await _employeeRepository.GetByIdAsync(userId);
            if (employee == null)
            {
                employee = await AddNewUserFromImport(message, company, cvPartnerImportData);
            }

            employee.ImportData(company.Id, company.Name, cvPartnerImportData, message.CreatedBy, message.CorrelationId);
            await _employeeRepository.SaveAsync(employee, Constants.IgnoreVersion);
        }

        private async Task<Employee> AddNewUserFromImport(ImportDataFromCvPartner message, Company company,
            CvPartnerImportData cvPartnerImportData)
        {
            var employee = new Employee();
            employee.CreateNew(company.Id, company.Name, IdService.CreateNewId(),
                new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email),
                cvPartnerImportData.FirstName, cvPartnerImportData.MiddleName, cvPartnerImportData.LastName,
                message.CreatedBy, message.CorrelationId);

            company.AddNewEmployeeToCompany(employee, message.CreatedBy, message.CorrelationId);
            await _companyRepository.SaveAsync(company, Constants.IgnoreVersion);
            return employee;
        }

        private async Task RemoveUsersNotInCvPartnerAnyMore(ImportDataFromCvPartner message, Company company, IEnumerable<CvPartnerImportData> importData)
        {
            var userIdList = company.GetAllUserIdsForUsersNotInList(importData);
            foreach (var userId in userIdList)
            {
                var employee = await _employeeRepository.GetByIdAsync(userId);
                company.RemoveEmployee(employee, message.CreatedBy, message.CorrelationId);
            }
        }
    }
}
