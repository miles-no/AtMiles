﻿using System;
using System.IO;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Import.CvPartner.CvPartner;
using Contact.Infrastructure.SingleServer.Properties;
using Contact.ReadStore;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Contact.ReadStore.UserStore;
using Newtonsoft.Json;

namespace Contact.Infrastructure.SingleServer
{
    class Program
    {
        

        static void Main(string[] args)
        {
            LongRunningProcess commandWorker;
            LongRunningProcess readStoreWorker;

            var configFilename = Settings.Default.ConfigFile;
            var config = GetConfig(configFilename);
            commandWorker = StartCommandHandler(config);
            readStoreWorker = StartReadModelHandler(config);
            Console.WriteLine("Press enter to finish");
            Console.ReadLine();
            Console.WriteLine("Stopping.......");
            commandWorker.Stop();
            readStoreWorker.Stop();
            Console.WriteLine("Stopped");
        }

        private static Config GetConfig(string configFilename)
        {
            if (!File.Exists(configFilename)) throw new FileNotFoundException();

            var raw = File.ReadAllText(configFilename);
            var config = JsonConvert.DeserializeObject<Config>(raw);

            return config;
        }

        private static LongRunningProcess StartCommandHandler(Config config)
        {
            var companyRepository = new EventStoreRepository<Company>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
            var employeeRepository = new EventStoreRepository<Employee>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
            var globalRepository = new EventStoreRepository<Global>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
            var commandSessionRepository = new EventStoreRepository<CommandSession>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);

            var importer = new ImportMiles(config.CvPartnerToken);
            var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository, globalRepository, importer);
            var cmdReceiver = new RabbitMqCommandHandler(cmdHandler, commandSessionRepository);

            var logger = new ConsoleLogger();
            var worker = new QueueWorker(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqQueueName, logger, cmdReceiver.MessageHandler);

            worker.Start();

            Console.WriteLine("Command-worker Started");
            return worker;
        }

        private static LongRunningProcess StartReadModelHandler(Config config)
        {
            var handlers = new ReadModelHandler();
            var store = RavenDocumentStore.CreateStore(config.RavenDbUrl);
            new EmployeeSearchStore(store).PrepareHandler(handlers);
            new CommandStatusStore(store).PrepareHandler(handlers);
            new UserLookupStore(new UserLookupEngine(store), store).PrepareHandler(handlers);
            var positionSaver = new PositionSaver(store);
            var read = new EventStoreDispatcher(config.EventServerHost, config.EventServerUsername, config.EventServerPassword, handlers, new ConsoleLogger(), () => { }, positionSaver);
            read.Start();

            return read;
        }
    }
}
