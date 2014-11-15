open no.miles.at.Backend.Domain
open no.miles.at.Backend.Domain.Commands
open no.miles.at.Backend.Domain.ValueTypes
open no.miles.at.Backend.Domain.Services
open no.miles.at.Backend.Infrastructure
open System
open System.Configuration

let CvPartnerSettingsKey = "RunImportFromCvPartner"
let Auth0SettingsKey = "RunImportFromAuth0"

let ReadSetting (key:string) =
    let appSettings = ConfigurationManager.AppSettings
    let value = appSettings.Get(key)
    match value with
        | "true" -> true
        | "True" -> true
        | _ -> false

let CreateCvPartnerCommand companyId =
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let correlationId = IdService.CreateNewId()
    new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)

let CreateAuth0Command companyId =
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let correlationId = IdService.CreateNewId()
    new EnrichFromAuth0(companyId, System.DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)

let ImportData (sender:RabbitMqCommandSender) (companyId:string) (logger:ILog) commandCreator loggerInfo key =
    match ReadSetting(key) with
        | true ->
            logger.Info(loggerInfo + " starting")
            let cmd = commandCreator companyId
            try
                sender.Send(cmd)
                logger.Info(loggerInfo + " completed")
            with
                | ex -> logger.Error("Error starting import from " + loggerInfo, ex)
            None
        | false ->
            logger.Info(loggerInfo + ": Not running")
            None

[<EntryPoint>]
let main argv =   
    let config = Configuration.ConfigManager.GetConfigUsingDefaultConfigFile()

    let logger = no.miles.at.Backend.Infrastructure.EventLogger("MilesSource", "AtMilesLog")
    use sender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger)

    ImportData sender config.CompanyId logger CreateAuth0Command "Auth0" Auth0SettingsKey |> ignore
    ImportData sender config.CompanyId logger CreateCvPartnerCommand "CvPartner" CvPartnerSettingsKey |> ignore
    0