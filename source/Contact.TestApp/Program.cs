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
            LongRunningProcess cmdWorker = null;
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
            const string officeId = "Stavanger";
            const string officeName = "Stavanger";
            var officeAddress = new Address("Øvre Holmegate 1, 3. etasje", "4006", "Stavanger");

            const string initCorrelationId = "SYSTEM INIT";
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);

            var admins = new List<SimpleUserInfo>();

            var admin1 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Roy", string.Empty, "Veshovda",
                new Login(Constants.GoogleIdProvider, "roy.veshovda@miles.no", string.Empty));
            admins.Add(admin1);

            var admin2 = new SimpleUserInfo(Domain.Services.IdService.CreateNewId(), "Stian", string.Empty, "Edvardsen",
                new Login(Constants.GoogleIdProvider, "stian.edvardsen@miles.no", string.Empty));
            admins.Add(admin2);

            var seedCommand = new AddNewCompanyToSystem(companyId, companyName, officeId, officeName, officeAddress, admins.ToArray(),
                DateTime.UtcNow, systemAsPerson, initCorrelationId, Constants.IgnoreVersion);

            var importCommand = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson,
                initCorrelationId, Constants.IgnoreVersion);

            //TODO: Rewrite to use RabbitMQ and/or webApi to seed.
            //Maybe RabbitMQ to send seedCommand, since we do not wish to expose this command.
            //And then WebAPI to issue Import-command

            var sender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername,
                config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl);
            
            sender.Send(seedCommand);
            sender.Send(importCommand);
        }
    }
}
