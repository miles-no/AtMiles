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

let ImportDataFromCvPartner(sender:RabbitMqCommandSender, companyId:string, logger:ILog) =
    logger.Info("CvPartner starting")
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let correlationId = IdService.CreateNewId()
    let cmd = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)
    try
        sender.Send(cmd)
        logger.Info("CvPartner completed")
    with
        | ex -> logger.Error("Error starting import from CV-Partner.", ex)
    None

let ImportDataFromAuth0(sender:RabbitMqCommandSender, companyId:string, logger:ILog) =
    logger.Info("Auth0 starting")
    let systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId)
    let correlationId = IdService.CreateNewId()
    let cmd = new EnrichFromAuth0(companyId, System.DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion)
    try
        sender.Send(cmd)
        logger.Info("Auth0 completed")
    with
        | ex -> logger.Error("Error starting import from Auth0.", ex)
    None

[<EntryPoint>]
let main argv =   
    let config = Configuration.ConfigManager.GetConfigUsingDefaultConfigFile()

    let runImportFromCvPartner = ReadSetting(CvPartnerSettingsKey)
    let runImportAuth0 = ReadSetting(Auth0SettingsKey)

    let logger = no.miles.at.Backend.Infrastructure.EventLogger("MilesSource", "AtMilesLog")
    use sender = new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName, config.RabbitMqUseSsl, logger)

    if runImportAuth0
        then ImportDataFromAuth0(sender, config.CompanyId, logger) |> ignore

    if runImportFromCvPartner
        then ImportDataFromCvPartner(sender, config.CompanyId, logger) |> ignore
    0