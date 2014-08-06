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
            var company = _companyRepository.GetById(Constants.SingleCompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier)) throw new Exception("Not allowed to");

            var employeeToBeAdmin = _employeeRepository.GetById(message.NewAdminId);
            if(employeeToBeAdmin == null) throw new Exception("Unknown user");


            company.AddCompanyAdmin(employeeToBeAdmin);
        }

        public void Handle(RemoveCompanyAdmin message)
        {
            var company = _companyRepository.GetById(Constants.SingleCompanyId);
            if (!company.IsCompanyAdmin(message.CreatedBy.Identifier)) throw new Exception("Not allowed to");

            var employeeToBeRemoved = _employeeRepository.GetById(message.AdminId);
            if (employeeToBeRemoved == null) throw new Exception("Unknown user");
            //TODO: Implement

            company.RemoveCompanyAdmin(employeeToBeRemoved);
        }

        public void Handle(RemoveOfficeAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(AddOfficeAdmin message)
        {
            //TODO: Implement
        }

        public void Handle(OpenOffice message)
        {
            //TODO: Implement
        }

        public void Handle(CloseOffice message)
        {
            //TODO: Implement
        }

        public void Handle(AddEmployee message)
        {
            //TODO: Implement
        }

        public void Handle(TerminateEmployee message)
        {
            //TODO: Implement
        }
    }
}
