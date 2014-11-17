open System
open no.miles.at.Backend.Domain
open no.miles.at.Backend.Domain.Commands
open no.miles.at.Backend.Domain.ValueTypes
open no.miles.at.Backend.Domain.Services
open no.miles.at.Backend.Infrastructure
open no.miles.at.Backend.Infrastructure.Configuration
open WorkerService

let printPoster workerRunning (message: string) = 
    Console.Clear()
    Console.WriteLine ""
    Console.Write "Result: "
    Console.WriteLine message
    Console.WriteLine ""
    Console.WriteLine "====================================================="
    Console.WriteLine "@Miles Tester"
    Console.WriteLine ""
    if workerRunning then
        Console.ForegroundColor <- ConsoleColor.Green
        Console.WriteLine " ** Worker running **"
        Console.ResetColor()
        Console.WriteLine ""

    Console.WriteLine "Select action:"
    Console.WriteLine "<i>: Information"
    Console.WriteLine "<1>: Start worker"
    Console.WriteLine "<2>: Stop worker"
    Console.WriteLine "<3>: Prepare EventStore with seed data"
    Console.WriteLine "<4>: Import data from CV-partner"
    Console.WriteLine "<5>: Import data from Auth0"
    Console.WriteLine "<Q>: Quit"
    Console.WriteLine "====================================================="
    Console.WriteLine "Select action:"

let startWorker (worker:WorkerService.WorkerProcess) running = 
    match running with
    | true -> (true, true, "Worker already running")
    | false ->
        worker.Start()
        (true, true, "Started worker")

let stopWorker (worker:WorkerService.WorkerProcess) running = 
    match running with
    | false -> (false, true, "Worker already stopped")
    | true ->
        Console.WriteLine "Stopping..."
        worker.Stop()
        (false, true, "Stopped worker")

let importFromCvPartner (config: Config)  (logger:ConsoleLogger) =
    
    None

let seedData running (logger : ILog) (config:Config) =
    let companyId = config.CompanyId
    let companyName = "Miles"
    let initCorrelationId = "INIT SEED"
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let admin1 = new SimpleUserInfo(IdService.CreateNewId(), "Roy", "", "Veshovda", new Login(Constants.GoogleIdProvider, "roy.veshovda@miles.no"))
    let admin2 = new SimpleUserInfo(IdService.CreateNewId(), "Stian", "", "Edvardsen", new Login(Constants.GoogleIdProvider, "stian.edvardsen@miles.no"))
    let admins = [|admin1;admin2|]

    let seedCommand = new AddNewCompanyToSystem(companyId, companyName, admins, DateTime.UtcNow, systemAsPerson, initCorrelationId, Constants.IgnoreVersion)
    use commandSender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger)
    commandSender.Send seedCommand
    (running, true, "Seeded data")

let importCvPartner running (logger : ILog) (config: Config)=
    let companyId = config.CompanyId
    let correlationId = IdService.CreateNewId()
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let importCommand = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)    
    use commandSender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger)
    commandSender.Send importCommand
    (running, true, "Imported from CV-partner")

let importAuth0 running (logger : ILog) (config:Config) =
    let companyId = config.CompanyId
    let correlationId = IdService.CreateNewId()
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let importCommand = new EnrichFromAuth0(companyId, DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)    
    use commandSender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger)
    commandSender.Send importCommand
    (running, true, "Imported from Auth0")

let resolveAction config worker logger running key = 
    match key with
    | ConsoleKey.D1 -> startWorker worker running
    | ConsoleKey.NumPad1 -> startWorker worker running     
    | ConsoleKey.D2 -> stopWorker worker running
    | ConsoleKey.NumPad2 -> stopWorker worker running
    | ConsoleKey.D3 -> seedData running logger config
    | ConsoleKey.NumPad3 -> seedData running logger config
    | ConsoleKey.D4 -> importCvPartner running logger config
    | ConsoleKey.NumPad4 -> importCvPartner running logger config
    | ConsoleKey.D5 -> importAuth0 running logger config
    | ConsoleKey.NumPad5 -> importAuth0 running logger config
    | ConsoleKey.Q -> (running, false, "Quit")
    | ConsoleKey.I -> (running, true, "Information")
    | _ -> (running, true, "Unknown command")

let rec loop config (worker:WorkerService.WorkerProcess) logger running message =
    printPoster running message
    let key = Console.ReadKey true
    let (r, l, mes) = resolveAction config worker logger running key.Key
    match l with
    | true -> loop config worker logger r mes
    | false ->
        Console.WriteLine "Stopping..."
        worker.Stop()
        None

[<EntryPoint>]
let main argv = 
    argv |> ignore
    let config = Configuration.ConfigManager.GetConfigUsingDefaultConfigFile()
    let logger = new ConsoleLogger();
    let worker = new WorkerProcess(logger);
    loop config worker logger false "" |> ignore
    0 // return an integer exit code
