using System;
using System.Threading.Tasks;
using Import.Auth0;
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

namespace WorkerService
{
    public class WorkerProcess
    {
        private LongRunningProcess _commandWorker;
        private LongRunningProcess _readStoreWorker;
        private readonly ILog _logger;

        public WorkerProcess(ILog logger)
        {
            if (logger == null) throw new Exception("Logger cannot be null");
            _logger = logger;
        }

        public void Start()
        {
            var config = ConfigManager.GetConfigUsingDefaultConfigFile();
            var t1 = StartCommandHandler(config, _logger);
            var t2 = StartReadModelHandler(config, _logger);
            t1.Wait();
            t2.Wait();
            _commandWorker = t1.Result;
            _readStoreWorker = t2.Result;
        }

        public void Stop()
        {
            if (_commandWorker != null) _commandWorker.Stop();
            if (_readStoreWorker != null) _readStoreWorker.Stop();
        }

        private static async Task<LongRunningProcess> StartCommandHandler(Config config, ILog logger)
        {
            LongRunningProcess worker = null;
            try
            {
                var companyRepository = new EventStoreRepository<Company>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
                var employeeRepository = new EventStoreRepository<Employee>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
                var globalRepository = new EventStoreRepository<Global>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);
                var commandSessionRepository = new EventStoreRepository<CommandSession>(config.EventServerHost, null, config.EventServerUsername, config.EventServerPassword);

                var importer = new ImportMiles(config.CvPartnerToken, logger);
                var enricher = new GetUsersFromAuth0(config.Auth0Audience, config.Auth0Secret, "https://atmiles.auth0.com");
                var cmdHandler = MainCommandHandlerFactory.Initialize(companyRepository, employeeRepository, globalRepository, importer, enricher);
                var cmdReceiver = new RabbitMqCommandHandler(cmdHandler, commandSessionRepository, logger);

                worker = new QueueWorker(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandQueueName, logger, cmdReceiver.MessageHandler);

                await worker.Start();
                logger.Info("CommandWorker started");
            }
            catch (Exception error)
            {
                logger.Error("Error starting CommandHandler", error);
            }
            return worker;
        }

        private static async Task<LongRunningProcess> StartReadModelHandler(Config config, ILog logger)
        {
            LongRunningProcess reader = null;
            try
            {
                var handlers = new ReadModelHandler();
                var store = RavenDocumentStore.CreateStore(config.RavenDbUrl);
                new EmployeeSearchStore(store).PrepareHandler(handlers);
                new CommandStatusStore(store).PrepareHandler(handlers);
                new UserLookupStore(store).PrepareHandler(handlers);
                new BusyTimeStore(store).PrepareHandler(handlers);
                var positionSaver = new PositionSaver(store);
                reader = new EventStoreDispatcher(config.EventServerHost, config.EventServerUsername,
                    config.EventServerPassword, handlers, new ConsoleLogger(), () => { }, positionSaver);
                await reader.Start();
                logger.Info("ReadStoreWorker started");
            }
            catch (Exception error)
            {
                logger.Error("Error starting ReadmodelHandler", error);
            }
            return reader;
        }
    }
}
