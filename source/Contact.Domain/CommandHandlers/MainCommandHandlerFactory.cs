using Contact.Domain.Aggregates;

namespace Contact.Domain.CommandHandlers
{
    public class MainCommandHandlerFactory
    {
        public static MainCommandHandler Initialize(IRepository<Company> repositoryCompany, IRepository<Employee> repositoryEmployee, IImportDataFromCvPartner cvPartnerImporter)
        {
            var cmdHandler = new MainCommandHandler();

            var cmdHandlerCompany = new CompanyCommandHandler(repositoryCompany, repositoryEmployee, cvPartnerImporter);

            cmdHandler.RegisterHandler<Commands.AddCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.AddEmployee>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.AddOfficeAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.CloseOffice>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.OpenOffice>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.RemoveCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.RemoveOfficeAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.TerminateEmployee>(cmdHandlerCompany.Handle);

            return cmdHandler;
        }
    }
}
