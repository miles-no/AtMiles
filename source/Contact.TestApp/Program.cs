using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Contact.Domain;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner;
using Contact.Infrastructure;
using Contact.ReadStore.Test;
using Contact.ReadStore.Test.SearchStore;
using Company = Contact.Domain.Aggregates.Company;
using Employee = Contact.Domain.Aggregates.Employee;

namespace Contact.TestApp
{
    class Program
    {
        static void Main()
        {
            LongRunningProcess cmdWorker = null;
            bool quit = false;

            while (!quit)
            {
                Console.WriteLine("");
                Console.WriteLine("=====================================================");
                Console.WriteLine("Miles Contact TestApp");
                Console.WriteLine("Select action:");
                Console.WriteLine("Quit: <q>");
                Console.WriteLine("Prepare EventStore with initial data: <I>");
                Console.WriteLine("Start command-handler: <C>");
                Console.WriteLine("Stop command-handler: <S>");
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
                        StopCommandHandler(cmdWorker);
                        quit = true;
                        break;
                    case ConsoleKey.I:
                        SeedEmptyEventStore().Wait();
                        break;
                    case ConsoleKey.C:
                        cmdWorker = StartCommandHandler();
                        break;
                    case ConsoleKey.S:
                        StopCommandHandler(cmdWorker);
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
                        var res = engine.FulltextSearch(query, 10, 0, out total);
                        Console.WriteLine(total + " treff ");
                        foreach (var personSearchModel in res)
                        {

                            Console.WriteLine(personSearchModel.Name + " Score: " + personSearchModel.Score);
                        }
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

            var companyRepository = new EventStoreRepository<Company>(host, null, eSUsername, eSPassword);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, eSUsername, eSPassword);
            var globalRepository = new EventStoreRepository<Global>(host, null, eSUsername, eSPassword);
            var commandSessionRepository = new EventStoreRepository<CommandSession>(host, null, eSUsername, eSPassword);


            var importer = new ImportMiles(GetCvPartnerToken());

            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository, globalRepository, importer);

            var cmdReceiver = new RabbitMqCommandHandler(cmdHandler, commandSessionRepository);

            var worker = new QueueWorker(host, rMqUsername, rMqPassword, "Commands", null, cmdReceiver.MessageHandler);

            worker.Start();

            Console.WriteLine("Command-worker Started");
            return worker;
        }

        private static string GetCvPartnerToken()
        {
            string cvPartnerToken;
    #if testing
            cvPartnerToken = File.ReadAllText("D:\\miles\\key.txt");
    #endif
            return cvPartnerToken;
        }

        private static async Task SeedEmptyEventStore()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            //const string password = "GoGoMilesContact";
            const string password = "changeit";


            const string companyId = "miles";
            const string companyName = "Miles";
            const string officeId = "Stavanger";
            const string officeName = "Stavanger";
            var officeAddress = new Address("Øvre Holmegate 1, 3. etasje", "4006", "Stavanger");

            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var globalRepository = new EventStoreRepository<Global>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);

            var importer = new ImportMiles(GetCvPartnerToken());

            var globalCommandHandler = new GlobalCommandHandler(companyRepository, employeeRepository, globalRepository, importer);


            const string initCorrelationId = "SYSTEM INIT";

            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);


            var admins = new List<SimpleUserInfo>();

            var admin1 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Roy", string.Empty, "Veshovda",
                new Login(Constants.GoogleIdProvider, "roy.veshovda@miles.no", string.Empty));
            admins.Add(admin1);

            var admin2 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Stian", string.Empty, "Edvardsen",
                new Login(Constants.GoogleIdProvider, "stian.edvardsen@miles.no", string.Empty));
            admins.Add(admin2);

            var seedCommand = new AddNewCompanyToSystem(companyId, companyName, officeId, officeName, officeAddress,admins.ToArray(),
                DateTime.UtcNow, systemAsPerson, initCorrelationId, Constants.IgnoreVersion);

            var importCommand = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson,
                initCorrelationId, Constants.IgnoreVersion);


            globalCommandHandler.Handle(seedCommand);
            globalCommandHandler.Handle(importCommand);
        }
    }
}
