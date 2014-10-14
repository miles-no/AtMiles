using System;

namespace no.miles.at.Backend.Api.Models.Api.Status
{
    public class StatusResponse
    {
        public string Id { get; set; }
        public string CommandName { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
    }
}