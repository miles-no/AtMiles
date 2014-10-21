using System;

namespace no.miles.at.Backend.ReadStore.SessionStore
{
    public class CommandStatus
    {
        public string Id { get; set; }
        public string CommandName { get; set; }
        public string Status { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Finished { get; set; }
    }
}