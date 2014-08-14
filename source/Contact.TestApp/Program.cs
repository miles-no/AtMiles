using System;
using Contact.Domain;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;

namespace Contact.TestApp
{
    class Program
    {
        static void Main(string[] args)
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
                Console.WriteLine("Prepare EventStore with initial data: <1>");
                Console.WriteLine("Start command-handler: <2>");
                Console.WriteLine("Stop command-handler: <3>");

                var key = Console.ReadKey(true);

                Console.Clear();
                Console.WriteLine("Result:");
                Console.WriteLine("");
                switch (key.Key)
                {

                    case ConsoleKey.Q:
                        Console.WriteLine("Quiting...");
                        quit = true;
                        break;
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:
                        Test1();
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:
                        cmdWorker = Test2();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Test3(cmdWorker);
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

        private static void Test3(LongRunningProcess worker)
        {
            if (worker != null)
            {
                worker.Stop();
                Console.WriteLine("Command-worker Stopped");
            }
        }

        private static LongRunningProcess Test2()
        {
            const string host = "milescontact.cloudapp.net";
            const string eSUsername = "admin";
            const string eSPassword = "changeit";

            const string rMQUsername = "miles";
            const string rMQPassword = "GoGoMilesContact";

            var companyRepository = new EventStoreRepository<Company>(host, null, eSUsername, eSPassword);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, eSUsername, eSPassword);

            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository);

            var cmdReceiver = new RabbitMqCommandHandler(cmdHandler);

            var worker = new QueueWorker(host, rMQUsername, rMQPassword, "Commands", null, cmdReceiver.MessageHandler);

            worker.Start();

            Console.WriteLine("Command-worker Started");
            return worker;
        }

        private static void Test1()
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

            const string admin1Id = "Google" + Constants.IdentitySeparator + "114551968215191716757";
            const string admin1FirstName = "Roy";
            const string admin1LastName = "Veshovda";
            var admin1DateOfBirth = new DateTime(1977, 1, 7);
            const string admin1JobTitle = "Senior Consultant";
            const string admin1PhoneNumber = "+4740102040";
            const string admin1Email = "roy.veshovda@miles.no";
            var admin1Address = new Address("Korvettveien 7", "4374", "Egersund");


            const string admin2Id = "Google" + Constants.IdentitySeparator + "110095646841016563805";
            const string admin2FirstName = "Stian";
            const string admin2LastName = "Galapate-Edvardsen";
            var admin2DateOfBirth = new DateTime(1977, 1, 7);
            const string admin2JobTitle = "Senior Consultant";
            const string admin2PhoneNumber = "+4712345678";
            const string admin2Email = "stian.edvardsen@miles.no";
            Address admin2Address = null;


            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var globalRepository = new EventStoreRepository<Global>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);
            var global = new Global();
            var admin1 = new Employee();
            admin1.CreateNew(companyId, companyName, officeId, officeName, admin1Id, admin1FirstName, string.Empty, admin1LastName, admin1DateOfBirth, admin1JobTitle, admin1PhoneNumber, admin1Email, admin1Address, null, new Person("SYSTEM", "SYSTEM"), "SYSTEM");

            var admin2 = new Employee();
            admin2.CreateNew(companyId, companyName, officeId, officeName, admin2Id, admin2FirstName, string.Empty, admin2LastName, admin2DateOfBirth, admin2JobTitle, admin2PhoneNumber, admin2Email, admin2Address, null, new Person("SYSTEM", "SYSTEM"), "SYSTEM");

            var company = new Company();
            company.CreateNewCompany(companyId, companyName, officeId, officeName, officeAddress, admin1.Id, admin1.Name, DateTime.UtcNow, new Person("SYSTEM", "SYSTEM"), "SYSTEM");
            company.AddCompanyAdmin(admin2, new Person("SYSTEM", "SYSTEM"), "SYSTEM");

            global.AddCompany(company, new Person("SYSTEM", "SYSTEM"), "SYSTEM");

            try
            {
                globalRepository.Save(global, Constants.NewVersion);
                employeeRepository.Save(admin1, Constants.NewVersion);
                employeeRepository.Save(admin2, Constants.NewVersion);
                companyRepository.Save(company, Constants.NewVersion);
                Console.WriteLine("Success!");
            }
            catch (Exception error)
            {
                Console.WriteLine("Exception: " + error);
            }
            
        }
    }
}
