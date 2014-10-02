using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain.Aggregates;
using Contact.Domain.Commands;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.CommandHandlers
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
                Constants.SystemUserId, null, string.Empty, string.Empty, Constants.SystemUserId,
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
                        adminId = Services.IdService.CreateNewId();
                    }

                    string email = string.Empty;
                    if (adminInfo.LoginId != null)
                    {
                        email = adminInfo.LoginId.Email;
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
                //TODO: Remove users not in CV-Partner anymore
                foreach (var cvPartnerImportData in importData)
                {
                    string userId = company.GetUserIdByLoginId(new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email, string.Empty));
                    var employee = await _employeeRepository.GetByIdAsync(userId);

                    if (employee == null)
                    {
                        employee = new Employee();
                        employee.CreateNew(company.Id, company.Name, Services.IdService.CreateNewId(),
                            new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email, string.Empty),
                            cvPartnerImportData.FirstName, cvPartnerImportData.MiddleName, cvPartnerImportData.LastName,
                            message.CreatedBy, message.CorrelationId);

                        company.AddNewEmployeeToCompany(employee, message.CreatedBy, message.CorrelationId);
                        await _companyRepository.SaveAsync(company, Constants.IgnoreVersion);
                    }

                    employee.ImportData(company.Id, company.Name, cvPartnerImportData, message.CreatedBy, message.CorrelationId);

                    await _employeeRepository.SaveAsync(employee, Constants.IgnoreVersion);
                }
            }
        }
    }
}
