using System;

namespace no.miles.at.Backend.Api.Models.Api.Status
{
    public class StatusResponse
    {
        public string Id;
        public string CommandName;
        public string Url;
        public string Status;
        public string ErrorMessage;
        public DateTime Started;
        public DateTime? Finished;
    }
}