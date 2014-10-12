using System.Threading.Tasks;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Import.CvPartner.CvPartner;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Infrastructure.Configuration;
using no.miles.at.Backend.ReadStore;
using no.miles.at.Backend.ReadStore.BusyTimeStore;
using no.miles.at.Backend.ReadStore.SearchStore;
using no.miles.at.Backend.ReadStore.SessionStore;
using no.miles.at.Backend.ReadStore.UserStore;
using no.miles.at.Backend.Worker.Properties;

namespace no.miles.at.Backend.Worker
{
    class Program
    {
        

        static void Main()
        {
            var configFilename = Settings.Default.ConfigFile;
            var config = ConfigManager.GetConfig(configFilename);
            var t1 = StartCommandHandler(config);
            var t2 = StartReadModelHandler(config);
            t1.Wait();
            t2.Wait();
            var commandWorker = t1.Result;
            var readStoreWorker = t2.Result;
            System.Console.WriteLine("Press enter to finish");
            System.Console.ReadLine();
            System.Console.WriteLine("Stopping.......");
            commandWorker.Stop();
            readStoreWorker.Stop();
            System.Console.WriteLine("Stopped");
        }

        private async static Task<LongRunningProcess> StartCommandHandler(Config config)
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

            await worker.Start();

            System.Console.WriteLine("Command-worker Started");
            return worker;
        }

        private async static Task<LongRunningProcess> StartReadModelHandler(Config config)
        {
            var handlers = new ReadModelHandler();
            var store = RavenDocumentStore.CreateStore(config.RavenDbUrl);
            new EmployeeSearchStore(store).PrepareHandler(handlers);
            new CommandStatusStore(store).PrepareHandler(handlers);
            new UserLookupStore(store).PrepareHandler(handlers);
            new BusyTimeStore(store).PrepareHandler(handlers);
            var positionSaver = new PositionSaver(store);
            var read = new EventStoreDispatcher(config.EventServerHost, config.EventServerUsername, config.EventServerPassword, handlers, new ConsoleLogger(), () => { }, positionSaver);
            await read.Start();

            return read;
        }
    }
}
