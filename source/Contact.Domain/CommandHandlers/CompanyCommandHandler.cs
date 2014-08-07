using System;
using Contact.Domain.Aggregates;
using Contact.Domain.Commands;

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
            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier)) throw new Exception("Not allowed to");

            var employeeToBeAdmin = _employeeRepository.GetById(message.NewAdminId);
            if(employeeToBeAdmin == null) throw new Exception("Unknown user");


            company.AddCompanyAdmin(employeeToBeAdmin);
            _companyRepository.Save(company,message.BasedOnVersion);
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier)) throw new Exception("Not allowed to");

            var employeeToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (employeeToBeRemoved == null) throw new Exception("Unknown user");
            //TODO: Implement

            company.RemoveCompanyAdmin(employeeToBeRemoved);
            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier) &&
                !company.IsOfficeAdmin(message.CreatedBy.Identifier, message.OfficeId))
            {
                throw new Exception("No access");

            }



            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddOfficeAdmin message)
        {
            var company = _companyRepository.GetById(message.CompanyId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(OpenOffice message)
        {
            var company = _companyRepository.GetById(message.CompanyId);

            company.IsCompanyAdmin(message.CreatedBy.Identifier);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(CloseOffice message)
        {
            var company = _companyRepository.GetById(message.CompanyId);
            company.IsCompanyAdmin(message.CreatedBy.Identifier);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddEmployee message)
        {
            var company = _companyRepository.GetById(message.CompanyId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(TerminateEmployee message)
        {
            var company = _companyRepository.GetById(message.CompanyId);

            _companyRepository.Save(company, message.BasedOnVersion);
        }
    }
}
