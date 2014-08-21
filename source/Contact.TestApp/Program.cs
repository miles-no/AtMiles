using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Contact.Domain;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner;
using Contact.Infrastructure;
using Contact.ReadStore.Test;
using Contact.ReadStore.Test.SearchStore;
using Contact.TestApp.InMemoryReadModel;
using Company = Contact.Domain.Aggregates.Company;
using Employee = Contact.Domain.Aggregates.Employee;

namespace Contact.TestApp
{
    class Program
    {
        static void Main()
        {
            LongRunningProcess cmdWorker = null;
            LongRunningProcess readModelDemo = null;
            bool quit = false;

            while (!quit)
            {
                Console.WriteLine("");
                Console.WriteLine("=====================================================");
                Console.WriteLine("Miles Contact TestApp");
                Console.WriteLine("Select action:");
                Console.WriteLine("Quit: <q>");
                Console.WriteLine("Prepare EventStore with initial data: <1>");
                Console.WriteLine("Start command-handler: <2>");
                Console.WriteLine("Stop command-handler: <3>");
                Console.WriteLine("ReadModel super-simple demo: <A>");
                Console.WriteLine("Import: <I>");
                Console.WriteLine("ReadModel demo: <R>");
                Console.WriteLine("Fill RavenDb read store demo: <F>");
                Console.WriteLine("Query RavenDb read store demo: <G>");

                var key = Console.ReadKey(true);

                Console.Clear();
                Console.WriteLine("Result:");
                Console.WriteLine("");
                switch (key.Key)
                {

                    case ConsoleKey.Q:
                        Console.WriteLine("Quiting...");
                        StopReadModel(readModelDemo);
                        quit = true;
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        SeedEmptyEventStore().Wait();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        cmdWorker = StartCommandHandler();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        StopCommandHandler(cmdWorker);
                        break;
                    case ConsoleKey.R:
                        Console.WriteLine("Starting Readmodel processing");
                        readModelDemo = ReadModelDemo();
                        break;
                    case ConsoleKey.F:
                        var admin = new ReadStoreAdmin();
                        admin.PrepareHandlers();
                        admin.StartListening();
                        break;

                    case ConsoleKey.G:

                        var engine = new EmployeeSearchEngine();
                        Console.WriteLine("Write query:");
                        var query = Console.ReadLine();
                        int total;
                        var res = engine.FulltextSearch(query,10, 0, out total);
                        Console.WriteLine(total + " treff ");
                        foreach (var personSearchModel in res)
                        {
                          
                            Console.WriteLine(personSearchModel.Name + " Score: " +  personSearchModel.Score);
                        }
                        break;

                    case ConsoleKey.S:
                        StopReadModel(readModelDemo);
                        break;

                    case ConsoleKey.I:
                        ImportFromMiles().Wait();
                        break;

                    //Add more functions here

                    default:
                        Console.WriteLine("Unknown key: " + key.Key);
                        break;
                }
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("Exited");
        }

        private static async Task ImportFromMiles()
        {
            const string host = "milescontact.cloudapp.net";
            const string eSUsername = "admin";
            const string eSPassword = "changeit";

            var employeeRepository = new EventStoreRepository<Employee>(host, null, eSUsername, eSPassword);
            string correlationId = "SYSTEM IMPORT: " + DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);

            var system = employeeRepository.GetById(Constants.SystemUserId);
            var systemAsPerson = new Person(system.Id, system.Name);

            string cvPartnerToken;
#if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
#endif

            var import = new ImportMiles(cvPartnerToken);
            var data = await import.GetImportData();
            int du = 0;
        }

        private static void StopReadModel(LongRunningProcess readModelDemo)
        {
            if (readModelDemo != null)
            {
                Console.WriteLine("Stopping readmodel...");
                readModelDemo.Stop();
                Console.WriteLine("ReadModel Stopped");
            }
        }


        private static void StopCommandHandler(LongRunningProcess worker)
        {
            if (worker != null)
            {
                worker.Stop();
                Console.WriteLine("Command-worker Stopped");
            }
        }

        private static LongRunningProcess StartCommandHandler()
        {
            const string host = "milescontact.cloudapp.net";
            const string eSUsername = "admin";
            const string eSPassword = "changeit";

            const string rMqUsername = "miles";
            const string rMqPassword = "GoGoMilesContact";

            string cvPartnerToken;
#if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
#endif


            var companyRepository = new EventStoreRepository<Company>(host, null, eSUsername, eSPassword);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, eSUsername, eSPassword);
            var commandSessionRepository = new EventStoreRepository<CommandSession>(host, null, eSUsername, eSPassword);

            
            var importer = new ImportMiles(cvPartnerToken);

            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository, importer);



            var cmdReceiver = new RabbitMqCommandHandler(cmdHandler, commandSessionRepository);

            var worker = new QueueWorker(host, rMqUsername, rMqPassword, "Commands", null, cmdReceiver.MessageHandler);

            worker.Start();

            Console.WriteLine("Command-worker Started");
            return worker;
        }

        private static async Task SeedEmptyEventStore()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            //const string password = "GoGoMilesContact";
            const string password = "changeit";

            const string companyId = "miles";
            const string companyName = "Miles";
            const string officeId = "SVG";
            const string officeName = "Stavanger";
            var officeAddress = new Address("Øvre Holmegate 1, 3. etasje", "4006", "Stavanger");

            const string systemId = Constants.SystemUserId;
            const string systemLastName = Constants.SystemUserId;
            var systemDateOfBirth = new DateTime(2014, 1, 1);
            const string systemJobTitle = Constants.SystemUserId;

            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var globalRepository = new EventStoreRepository<Global>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);
            var global = new Global();

            const string initCorrelationId = "SYSTEM INIT";

            var system = new Employee();
            system.CreateNew(companyId, companyName, officeId, officeName, systemId, null, string.Empty, string.Empty, systemLastName, systemDateOfBirth, systemJobTitle, string.Empty, String.Empty, null, null, null, new Person("SYSTEM", "SYSTEM"), initCorrelationId);

            var systemAsPerson = new Person(system.Id, system.Name);

            var company = new Company();
            company.CreateNewCompany(companyId, companyName, officeId, officeName, officeAddress, system.Id, system.Name, DateTime.UtcNow, systemAsPerson, initCorrelationId);

            global.AddCompany(company, systemAsPerson, initCorrelationId);

            try
            {
                globalRepository.Save(global, Constants.NewVersion);
                employeeRepository.Save(system, Constants.NewVersion);
                companyRepository.Save(company, Constants.NewVersion);
                Console.WriteLine("Successfully created seed info");
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception: " + error);
                return;
            }

            Console.WriteLine("Starting to import data from CVpartner");

            string cvPartnerToken;
#if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
#endif
            var import = new ImportMiles(cvPartnerToken);
            var companyCommandHandler = new CompanyCommandHandler(companyRepository, employeeRepository, import);

            
            var userEmailsToPromotoToCompanyAdmin = new List<string> {"roy.veshovda@miles.no", "stian.edvardsen@miles.no"};

            await import.ImportMilesComplete(systemAsPerson, initCorrelationId, companyCommandHandler.Handle, companyCommandHandler.Handle, companyCommandHandler.Handle, userEmailsToPromotoToCompanyAdmin);
        }

        private static LongRunningProcess ReadModelDemo()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            //const string password = "GoGoMilesContact";
            const string password = "changeit";

            var handler = new ReadModelHandler();

            var repository = new InMemoryRepository();

            var employeeHandler = new EmployeeHandler(repository);
            handler.RegisterHandler<EmployeeCreated>(employeeHandler.Handle);
            handler.RegisterHandler<EmployeeTerminated>(employeeHandler.Handle);

            var companyHandler = new CompanyHandler(repository);
            handler.RegisterHandler<CompanyCreated>(companyHandler.Handle);
            handler.RegisterHandler<CompanyAdminAdded>(companyHandler.Handle);
            handler.RegisterHandler<CompanyAdminRemoved>(companyHandler.Handle);
            handler.RegisterHandler<EmployeeAdded>(companyHandler.Handle);
            handler.RegisterHandler<EmployeeRemoved>(companyHandler.Handle);
            handler.RegisterHandler<OfficeAdminAdded>(companyHandler.Handle);
            
            handler.RegisterHandler<OfficeAdminRemoved>(companyHandler.Handle);
            handler.RegisterHandler<OfficeClosed>(companyHandler.Handle);
            handler.RegisterHandler<OfficeOpened>(companyHandler.Handle);

            var demo = new EventStoreDispatcher(host, username, password, handler, new ConsoleLogger(), () => PrintInMemoRyRepository(repository));
            demo.Start();

            return demo;
        }

        private static void PrintInMemoRyRepository(InMemoryRepository repository)
        {
            Console.WriteLine("ReadModel contains:");

            Console.WriteLine("Employees: {0}", repository.Employees.Count);

            Console.WriteLine("Companies: {0}",repository.Companies.Count);
            Console.WriteLine();
            foreach (var company in repository.Companies)
            {
                Console.WriteLine("Company: {0}:",company.Name);
                Console.WriteLine("\t{0} Admins", company.Admins.Count);
                Console.WriteLine("\t{0} Offices", company.Offices.Count);
                Console.WriteLine();
                foreach (var office in company.Offices)
                {
                    Console.WriteLine("\tOffice: {0}:", office.Name);
                    Console.WriteLine("\t\tAdmins: {0}:", office.Admins.Count);
                    Console.WriteLine("\t\tEmployees: {0}:", office.Employees.Count);
                }
            }
            Console.WriteLine();
            Console.WriteLine("***********************************************************");
        }
    }
}
