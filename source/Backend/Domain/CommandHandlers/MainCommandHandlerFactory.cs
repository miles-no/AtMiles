using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.Commands;

namespace no.miles.at.Backend.Domain.CommandHandlers
{
    public static class MainCommandHandlerFactory
    {
        public static MainCommandHandler Initialize(IRepository<Company> repositoryCompany, IRepository<Employee> repositoryEmployee, IRepository<Global> repositoryGlobal, IImportDataFromCvPartner cvPartnerImporter)
        {
            var cmdHandler = new MainCommandHandler();

            var globalCommandHandler = new GlobalCommandHandler(repositoryCompany, repositoryEmployee, repositoryGlobal, cvPartnerImporter);

            cmdHandler.RegisterHandler<ImportDataFromCvPartner>(globalCommandHandler.Handle);
            cmdHandler.RegisterHandler<EnrichFromAuth0>(globalCommandHandler.Handle);
            cmdHandler.RegisterHandler<EnrichEmployeeFromAuth0>(globalCommandHandler.Handle);
            
            cmdHandler.RegisterHandler<AddNewCompanyToSystem>(globalCommandHandler.Handle);


            var cmdHandlerCompany = new CompanyCommandHandler(repositoryCompany, repositoryEmployee);

            cmdHandler.RegisterHandler<AddCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<AddEmployee>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<RemoveCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<TerminateEmployee>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<AddBusyTime>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<RemoveBusyTime>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<ConfirmBusyTimeEntries>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<UpdateBusyTime>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<SetDateOfBirth>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<SetPrivateAddress>(cmdHandlerCompany.Handle);
            return cmdHandler;
        }
    }
}
