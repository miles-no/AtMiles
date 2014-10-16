using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Infrastructure.Configuration;

namespace no.miles.at.Backend.TestApp
{
    static class Program
    {
        private static RabbitMqCommandSender _commandSender;
        static void Main()
        {
            bool quit = false;
            var config = ConfigManager.GetConfigUsingDefaultConfigFile();
            var logger = new ConsoleLogger();

            while (!quit)
            {
                Console.WriteLine("");
                Console.WriteLine("=====================================================");
                Console.WriteLine("Miles Contact TestApp");
                Console.WriteLine("Select action:");
                Console.WriteLine("Quit: <q>");
                Console.WriteLine("Prepare EventStore with initial data: <S>");
                Console.WriteLine("Import from CVPartner: <I>");
                var key = Console.ReadKey(true);

                Console.Clear();
                Console.WriteLine("Result:");
                Console.WriteLine("");
                switch (key.Key)
                {

                    case ConsoleKey.Q:
                        Console.WriteLine("Quiting...");
                        if(_commandSender != null) _commandSender.Dispose();
                        quit = true;
                        break;
                    case ConsoleKey.I:
                        ImportFromCvPartner(config, logger).Wait();
                        break;
                    case ConsoleKey.S:
                        SeedEmptyEventStore(config, logger).Wait();
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

        private static async Task ImportFromCvPartner(Config config, ConsoleLogger logger)
        {
            string companyId = config.CompanyId;

            var correlationId = IdService.CreateNewId();
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);

            var importCommand = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson,
                correlationId, Constants.IgnoreVersion);

            _commandSender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername,
                config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger);

            await Task.Run(() => _commandSender.Send(importCommand));
            //TODO: wait for results to come back
        }

        private static async Task SeedEmptyEventStore(Config config, ILog logger)
        {
            string companyId = config.CompanyId;
            const string companyName = "Miles";

            const string initCorrelationId = "INIT SEED";
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);

            var admins = new List<SimpleUserInfo>();

            var admin1 = new SimpleUserInfo(IdService.CreateNewId(), "Roy", string.Empty, "Veshovda",
                new Login(Constants.GoogleIdProvider, "roy.veshovda@miles.no"));
            admins.Add(admin1);

            var admin2 = new SimpleUserInfo(IdService.CreateNewId(), "Stian", string.Empty, "Edvardsen",
                new Login(Constants.GoogleIdProvider, "stian.edvardsen@miles.no"));
            admins.Add(admin2);

            var seedCommand = new AddNewCompanyToSystem(companyId, companyName, admins.ToArray(),
                DateTime.UtcNow, systemAsPerson, initCorrelationId, Constants.IgnoreVersion);

            _commandSender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername,
                config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger);

            await Task.Run(() => _commandSender.Send(seedCommand));
            //TODO: wait for results to come back
        }
    }
}
