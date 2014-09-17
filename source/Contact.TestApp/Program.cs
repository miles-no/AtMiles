using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Contact.Infrastructure.Configuration;
using Contact.TestApp.Properties;

namespace Contact.TestApp
{
    class Program
    {
        static void Main()
        {
            bool quit = false;
            var config = ConfigManager.GetConfig(Settings.Default.ConfigFile);

            while (!quit)
            {
                Console.WriteLine("");
                Console.WriteLine("=====================================================");
                Console.WriteLine("Miles Contact TestApp");
                Console.WriteLine("Select action:");
                Console.WriteLine("Quit: <q>");
                Console.WriteLine("Prepare EventStore with initial data: <I>");
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
                    case ConsoleKey.I:
                        SeedEmptyEventStore(config).Wait();
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

        private static async Task SeedEmptyEventStore(Config config)
        {
            string companyId = config.CompanyId;
            const string companyName = "Miles";
            const string officeName = "Stavanger";

            const string initCorrelationId1 = "SYSTEM INIT 1";
            const string initCorrelationId2 = "SYSTEM INIT 2";
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);

            var admins = new List<SimpleUserInfo>();

            var admin1 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Roy", string.Empty, "Veshovda",
                new Login(Constants.GoogleIdProvider, "roy.veshovda@miles.no", string.Empty));
            admins.Add(admin1);

            var admin2 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Stian", string.Empty, "Edvardsen",
                new Login(Constants.GoogleIdProvider, "stian.edvardsen@miles.no", string.Empty));
            admins.Add(admin2);

            var seedCommand = new AddNewCompanyToSystem(companyId, companyName, officeName, admins.ToArray(),
                DateTime.UtcNow, systemAsPerson, initCorrelationId1, Constants.IgnoreVersion);

            var importCommand = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson,
                initCorrelationId2, Constants.IgnoreVersion);

            var sender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername,
                config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl);
            await Task.Run(() => sender.Send(seedCommand));
            await Task.Run(() => sender.Send(importCommand));

            //TODO: wait for results to come back
        }
    }
}
