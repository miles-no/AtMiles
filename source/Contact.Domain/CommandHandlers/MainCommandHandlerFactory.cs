using Contact.Domain.Aggregates;

namespace Contact.Domain.CommandHandlers
{
    public class MainCommandHandlerFactory
    {
        public static MainCommandHandler Initialize(IRepository<Company> repositoryCompany, IRepository<Employee> repositoryEmployee, IRepository<Global> repositoryGlobal, IImportDataFromCvPartner cvPartnerImporter)
        {
            var cmdHandler = new MainCommandHandler();

            var globalCommandHandler = new GlobalCommandHandler(repositoryCompany, repositoryEmployee, repositoryGlobal, cvPartnerImporter);

            cmdHandler.RegisterHandler<Commands.ImportDataFromCvPartner>(globalCommandHandler.Handle);
            cmdHandler.RegisterHandler<Commands.AddNewCompanyToSystem>(globalCommandHandler.Handle);


            var cmdHandlerCompany = new CompanyCommandHandler(repositoryCompany, repositoryEmployee);

            cmdHandler.RegisterHandler<Commands.AddCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.AddEmployee>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.AddOfficeAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.CloseOffice>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.OpenOffice>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.RemoveCompanyAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.RemoveOfficeAdmin>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.TerminateEmployee>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.MoveEmployeeToNewOffice>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.AddBusyTime>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.RemoveBusyTime>(cmdHandlerCompany.Handle);
            cmdHandler.RegisterHandler<Commands.ConfirmBusyTimeEntries>(cmdHandlerCompany.Handle);

            return cmdHandler;
        }
    }
}
