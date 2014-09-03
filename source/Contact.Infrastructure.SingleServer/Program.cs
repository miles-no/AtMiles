using Contact.Configuration;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Import.CvPartner.CvPartner;
using Contact.Infrastructure.SingleServer.Properties;
using Contact.ReadStore;
using Contact.ReadStore.SearchStore;
using Contact.ReadStore.SessionStore;
using Contact.ReadStore.UserStore;


namespace Contact.Infrastructure.SingleServer
{
    class Program
    {
        

        static void Main()
        {
            var configFilename = Settings.Default.ConfigFile;
            var config = ConfigManager.GetConfig(configFilename);
            var commandWorker = StartCommandHandler(config);
            var readStoreWorker = StartReadModelHandler(config);
            System.Console.WriteLine("Press enter to finish");
            System.Console.ReadLine();
            System.Console.WriteLine("Stopping.......");
            commandWorker.Stop();
            readStoreWorker.Stop();
            System.Console.WriteLine("Stopped");
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
            var worker = new QueueWorker(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandQueueName, logger, cmdReceiver.MessageHandler);

            worker.Start();

            System.Console.WriteLine("Command-worker Started");
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
