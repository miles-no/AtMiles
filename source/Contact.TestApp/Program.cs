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
            RabbitMqCommandHandler cmdHandler = null;
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
                        cmdHandler = Test2();
                        break;
                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:
                        Test3(cmdHandler);
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

        private static void Test3(RabbitMqCommandHandler cmdHandler)
        {
            if (cmdHandler != null)
            {
                cmdHandler.StopReceiving();
            }
        }

        private static RabbitMqCommandHandler Test2()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            const string password = "GoGoMilesContact";

            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);

            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository);
            
            RabbitMqCommandHandler cmdReceiver = new RabbitMqCommandHandler(cmdHandler);

            cmdReceiver.StartReceiving();

            return cmdReceiver;
        }

        private static void Test1()
        {
            const string host = "milescontact.cloudapp.net";
            const string username = "admin";
            const string password = "GoGoMilesContact";

            const string companyId = "miles";
            const string companyName = "Miles";
            const string officeId = "SVG";
            const string officeName = "Stavanger";
            var officeAddress = new Address("Øvre Holmegate 1, 3. etasje", "4006", "Stavanger");

            const string adminId = "Google::114551968215191716757";
            const string adminFirstName = "Roy";
            const string adminLastName = "Veshovda";
            var adminDateOfBirth = new DateTime(1977, 1, 7);
            const string adminJobTitle = "Senior Consultant";
            const string adminPhoneNumber = "+4740102040";
            const string adminEmail = "roy.veshovda@miles.no";
            var adminAddress = new Address("Korvettveien 7", "4374", "Egersund");


            var companyRepository = new EventStoreRepository<Company>(host, null, username, password);
            var globalRepository = new EventStoreRepository<Global>(host, null, username, password);
            var employeeRepository = new EventStoreRepository<Employee>(host, null, username, password);
            var global = new Global();
            var admin = new Employee();
            admin.CreateNew(companyId, companyName, officeId, officeName, adminId, adminFirstName, string.Empty, adminLastName, adminDateOfBirth, adminJobTitle, adminPhoneNumber, adminEmail, adminAddress, null, new Person("SYSTEM", "SYSTEM"), "SYSTEM");

            var company = new Company();
            company.CreateNewCompany(companyId, companyName, officeId, officeName, officeAddress, admin.Id, admin.Name);

            global.AddCompany(company);

            try
            {
                globalRepository.Save(global, Constants.NewVersion);
                employeeRepository.Save(admin, Constants.NewVersion);
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
