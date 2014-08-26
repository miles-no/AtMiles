using System;
using System.Collections.Generic;
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

        public void Handle(AddNewCompanyToSystem message)
        {
            var global = _globalRepository.GetById(Global.GlobalId);
            if(global == null) global = new Global();

            if(global.HasCompany(message.CompanyId)) throw new AlreadyExistingItemException("CompanyId already in system");

            var system = new Employee();
            system.CreateNew(message.CompanyId, message.CompanyName, message.FirstOfficeId, message.FirstOfficeName,
                Constants.SystemUserId, null, string.Empty, string.Empty, Constants.SystemUserId, DateTime.UtcNow,
                string.Empty, string.Empty, string.Empty, null, null,
                new Person(Constants.SystemUserId, Constants.SystemUserId), message.CorrelationId);


            _employeeRepository.Save(system, Constants.NewVersion);


            var systemAsPerson = new Person(system.Id, system.Name);

            var company = new Company();
            company.CreateNewCompany(message.CompanyId, message.CompanyName, message.FirstOfficeId, message.FirstOfficeName, message.FirstOfficeAddress, system.Id, system.Name, DateTime.UtcNow, systemAsPerson, message.CorrelationId);

            company.OpenOffice(message.FirstOfficeId, message.FirstOfficeName, message.FirstOfficeAddress, message.CreatedBy, message.CorrelationId);

            global.AddCompany(company, systemAsPerson, message.CorrelationId);

            _globalRepository.Save(global, Constants.NewVersion);
            _companyRepository.Save(company, Constants.NewVersion);

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

                    admin.CreateNew(message.CompanyId, message.CompanyName,message.FirstOfficeId, message.FirstOfficeName, adminId, adminInfo.LoginId,adminInfo.FirstName, adminInfo.MiddleName, adminInfo.LastName,null,string.Empty,string.Empty,email,null,null,message.CreatedBy, message.CorrelationId);
                    _employeeRepository.Save(admin, Constants.NewVersion);
                    company.AddNewEmployeeToOffice(message.FirstOfficeId, admin,message.CreatedBy, message.CorrelationId);
                    company.AddCompanyAdmin(admin, message.CreatedBy, message.CorrelationId);
                }
            }
            _companyRepository.Save(company, Constants.IgnoreVersion);
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

                    var office = company.GetOfficeByName(cvPartnerImportData.OfficeName);
                    if (office == null)
                    {
                        company.OpenOffice(cvPartnerImportData.OfficeName, cvPartnerImportData.OfficeName, null, message.CreatedBy, message.CorrelationId);
                        office = company.GetOffice(cvPartnerImportData.OfficeName);
                    }

                    if (employee == null)
                    {
                        employee = new Employee();
                        employee.CreateNew(company.Id, company.Name, office.Id, office.Name, Services.IdService.CreateNewId(), new Login(Constants.GoogleIdProvider, cvPartnerImportData.Email, string.Empty), cvPartnerImportData.FirstName, cvPartnerImportData.MiddleName, cvPartnerImportData.LastName, cvPartnerImportData.DateOfBirth, cvPartnerImportData.Title, cvPartnerImportData.Phone, cvPartnerImportData.Email, null, null, message.CreatedBy, message.CorrelationId);

                        company.AddNewEmployeeToOffice(office.Id, employee, message.CreatedBy, message.CorrelationId);
                        _companyRepository.Save(company, Constants.IgnoreVersion);
                    }

                    employee.ImportData(cvPartnerImportData, message.CreatedBy, message.CorrelationId);

                    _employeeRepository.Save(employee, Constants.IgnoreVersion);
                }
            }
        }
    }
}
