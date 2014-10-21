namespace no.miles.at.Backend.Infrastructure.Configuration
{
    public class Config
    {
        public string CompanyId { get; set; }
        public string StatusEndpointUrl { get; set; }
        public string EventServerHost { get; set; }
        public string EventServerUsername { get; set; }
        public string EventServerPassword { get; set; }
        public string RabbitMqHost { get; set; }
        public string RabbitMqUsername { get; set; }
        public string RabbitMqPassword { get; set; }
        public bool RabbitMqUseSsl { get; set; }
        public string RabbitMqCommandQueueName { get; set; }
        public string RabbitMqCommandExchangeName { get; set; }
        public string CvPartnerToken { get; set; }
        public string RavenDbUrl { get; set; }
        public string Auth0Issuer { get; set; }
        public string Auth0Audience { get; set; }
        public string Auth0Secret { get; set; }
    }
}
