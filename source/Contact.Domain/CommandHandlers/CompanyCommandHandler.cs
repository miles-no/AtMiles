using Contact.Domain.Aggregates;
using Contact.Domain.Commands;
using Contact.Domain.Exceptions;

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

            var company = _companyRepository.GetById(message.CompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier) &&
                !company.IsOfficeAdmin(message.CreatedBy.Identifier, message.OfficeId))
            {
                throw new NoAccessException();

            }

            //TODO: Implement

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(AddOfficeAdmin message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            //TODO: Implement

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(OpenOffice message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            if(!company.IsCompanyAdmin(admin.Id)) throw new NoAccessException();

            company.OpenOffice(message.Name, message.Address, message.CreatedBy, message.CorrelationId);

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

            //TODO: Implement

            _companyRepository.Save(company, message.BasedOnVersion);
        }

        public void Handle(TerminateEmployee message)
        {
            var admin = _employeeRepository.GetById(message.CreatedBy.Identifier);
            if (admin == null) throw new UnknownItemException();

            var company = _companyRepository.GetById(message.CompanyId);

            //TODO: Implement

            _companyRepository.Save(company, message.BasedOnVersion);
        }
    }
}
