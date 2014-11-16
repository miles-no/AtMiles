open System
//open no.miles.at.Backend.Infrastructure
//open WorkerService

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
    Console.WriteLine "<1>: Start worker"
    Console.WriteLine "<2>: Stop worker"
    Console.WriteLine "<3>: Prepare EventStore with seed data"
    Console.WriteLine "<4>: Import data from CV-partner"
    Console.WriteLine "<5>: Import data from Auth0"
    Console.WriteLine "<Q>: Quit"
    Console.WriteLine "====================================================="
    Console.Write "Select action:"

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
        worker.Stop()
        (false, true, "Stopped worker")

let seedData running =
    //TODO: Implement
    (running, true, "Seeded data")

let importCvPartner running =
    //TODO: Implement
    (running, true, "Imported from CV-partner")

let importAuth0 running =
    //TODO: Implement
    (running, true, "Imported from Auth0")

let resolveAction worker running key = 
    match key with
    | ConsoleKey.D1 -> startWorker worker running
    | ConsoleKey.NumPad1 -> startWorker worker running     
    | ConsoleKey.D2 -> stopWorker worker running
    | ConsoleKey.NumPad2 -> stopWorker worker running
    | ConsoleKey.D3 -> seedData running
    | ConsoleKey.NumPad3 -> seedData running
    | ConsoleKey.D4 -> importCvPartner running
    | ConsoleKey.NumPad4 -> importCvPartner running
    | ConsoleKey.D5 -> importAuth0 running
    | ConsoleKey.NumPad5 -> importAuth0 running
    | ConsoleKey.Q ->
        (running, false, "Quit")
    | _ ->
        (running, true, "Unknown command")

let rec loop (worker:WorkerService.WorkerProcess) running message =
    printPoster running message
    let key = Console.ReadKey true
    let (r, l, mes) = resolveAction worker running key.Key
    match l with
    | true -> loop worker r mes
    | false ->
        Console.WriteLine "Stopping..."
        worker.Stop()
        None

[<EntryPoint>]
let main argv = 
    argv |> ignore
    let logger = new no.miles.at.Backend.Infrastructure.ConsoleLogger();
    let worker = new WorkerService.WorkerProcess(logger);
    loop worker false "" |> ignore
    0 // return an integer exit code
