using System;
using System.Configuration;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.Infrastructure.Configuration;

namespace ScheduledRunner
{
    class Program
    {
        private const string CvPartnerSettingsKey = "RunImportFromCvPartner";
        private const string Auth0SettingsKey = "RunImportFromAuth0";

        static void Main()
        {
            var config = ConfigManager.GetConfigUsingDefaultConfigFile();
            var logger = new ConsoleLogger();

            var sender = GetSender(config, logger);

            var shouldRunImportFromCvPartner = ReadSetting(CvPartnerSettingsKey);
            if(shouldRunImportFromCvPartner) ImportDataFromCvPartner(sender, config.CompanyId, logger);

            var shouldRunImportFromAuth0 = ReadSetting(Auth0SettingsKey);
            if (shouldRunImportFromAuth0) ImportDataFromAuth0(sender, config.CompanyId, logger);

            sender.Dispose();
        }

        private static void ImportDataFromAuth0(RabbitMqCommandSender sender, string companyId, ILog logger)
        {
            logger.Info("Auth0 starting");
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);
            var correlationId = IdService.CreateNewId();
            //var cmd = new EnrichWithDataFromAuth0(config.CompanyId, DateTime.UtcNow, systemAsPerson, correlationId, Constants.IgnoreVersion);
            try
            {
                //sender.Send(cmd);
                logger.Info("Auth0 completed");
            }
            catch (Exception ex)
            {
                logger.Error("Error starting import from Auth0.", ex);
            }
        }

        private static void ImportDataFromCvPartner(RabbitMqCommandSender sender, string companyId, ILog logger)
        {
            logger.Info("CvPartner starting");
            var systemAsPerson = new Person(Constants.SystemUserId, Constants.SystemUserId);
            var correlationId = IdService.CreateNewId();
            var cmd = new ImportDataFromCvPartner(companyId,DateTime.UtcNow, systemAsPerson, correlationId,Constants.IgnoreVersion);
            try
            {
                sender.Send(cmd);
                logger.Info("CvPartner completed");
            }
            catch (Exception ex)
            {
                logger.Error("Error starting import from CV-Partner.", ex);
            }
        }

        private static RabbitMqCommandSender GetSender(Config config, ILog logger)
        {
            return new RabbitMqCommandSender(config.RabbitMqHost, config.RabbitMqUsername, config.RabbitMqPassword, config.RabbitMqCommandExchangeName,config.RabbitMqUseSsl, logger);
        }

        private static bool ReadSetting(string key)
        {
            var appSettings = ConfigurationManager.AppSettings;
            if (!appSettings.HasKeys()) return false;
            var val = appSettings[key];
            if (val == "True" || val == "true") return true;
            return false;
        }
    }
}
