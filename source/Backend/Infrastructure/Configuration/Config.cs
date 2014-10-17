namespace no.miles.at.Backend.Infrastructure.Configuration
{
    public class Config
    {
        public string CompanyId;
        public string StatusEndpointUrl;
        public string EventServerHost;
        public string EventServerUsername;
        public string EventServerPassword;
        public string RabbitMqHost;
        public string RabbitMqUsername;
        public string RabbitMqPassword;
        public bool RabbitMqUseSsl;
        public string RabbitMqCommandQueueName;
        public string RabbitMqCommandExchangeName;
        public string CvPartnerToken;
        public string RavenDbUrl;
        public string Auth0Issuer;
        public string Auth0Audience;
        public string Auth0Secret;
    }
}
