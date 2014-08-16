using System.Linq;
using System.Runtime.Remoting.Messaging;
using Contact.Domain;
using Contact.Domain.Events.Company;

namespace Contact.TestApp.InMemoryReadModel
{
    public class CompanyHandler :
        Handles<CompanyCreated>,
        Handles<CompanyAdminAdded>,
        Handles<CompanyAdminRemoved>,
        Handles<EmployeeAdded>,
        Handles<EmployeeRemoved>,
        Handles<OfficeAdminAdded>,
        Handles<OfficeAdminRemoved>,
        Handles<OfficeClosed>,
        Handles<OfficeOpened>
    {
        private readonly InMemoryRepository _repository;

        public CompanyHandler(InMemoryRepository repository)
        {
            _repository = repository;
        }

        public void Handle(CompanyCreated message)
        {
            var company = new Company { Id = message.CompanyId, Name = message.CompanyName };
            _repository.Companies.Add(company);
        }
        
        public void Handle(CompanyAdminAdded message)
        {
            var company = GetCompany(message.CompanyId);
            var newAdmin = new SimpleEmployee {Id = message.NewAdminId, Name = message.NewAdminName};
            company.AddCompanyAdmin(newAdmin);
        }

        public void Handle(CompanyAdminRemoved message)
        {
            var company = GetCompany(message.CompanyId);
            var admin = new SimpleEmployee { Id = message.AdminId, Name = message.AdminName };
            company.RemoveCompanyAdmin(admin);
        }

        public void Handle(EmployeeAdded message)
        {
            var company = GetCompany(message.CompanyId);
            var employee = new SimpleEmployee { Id = message.GlobalId, Name = message.Name };
            company.AddEmployeeToOffice(employee, message.OfficeId);
        }

        public void Handle(EmployeeRemoved message)
        {
            var company = GetCompany(message.CompanyId);
            company.RemoveEmployeeFromOffice(message.Id, message.OfficeId);
        }

        public void Handle(OfficeAdminAdded message)
        {
            var company = GetCompany(message.CompanyId);
            var admin = new SimpleEmployee { Id = message.AdminId, Name = message.AdminName };
            company.AddOfficeAdmin(message.OfficeId, admin);
        }

        public void Handle(OfficeAdminRemoved message)
        {
            var company = GetCompany(message.CompanyId);
            var admin = new SimpleEmployee { Id = message.AdminId, Name = message.AdminName };
            company.RemoveOfficeAdmin(message.OfficeId, admin);
        }

        public void Handle(OfficeClosed message)
        {
            var company = GetCompany(message.CompanyId);
            company.RemoveOffice(message.OfficeId);
        }

        public void Handle(OfficeOpened message)
        {
            var company = GetCompany(message.CompanyId);
            var office = new Office {Id = message.OfficeId, Name = message.OfficeName, Address = message.Address};
            company.AddOffice(office);
        }

        private Company GetCompany(string companyId)
        {
            return _repository.Companies.FirstOrDefault(c => c.Id == companyId);
        }
    }
}
