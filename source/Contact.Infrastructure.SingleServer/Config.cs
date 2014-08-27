﻿namespace Contact.Infrastructure.SingleServer
{
    public class Config
    {
        public string EventServerHost { get; set; }
        public string EventServerUsername { get; set; }
        public string EventServerPassword { get; set; }
        public string RabbitMqHost { get; set; }
        public string RabbitMqUsername { get; set; }
        public string RabbitMqPassword { get; set; }
        public string RabbitMqQueueName { get; set; }
        public string CvPartnerToken { get; set; }
        public string RavenDbUrl { get; set; }
    }
}