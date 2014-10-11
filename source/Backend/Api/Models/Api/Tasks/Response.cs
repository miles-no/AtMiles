using no.miles.at.Backend.Api.Models.Api.Status;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class Response
    {
        public string RequestId { get; set; }
        public StatusResponse Status { get; set; }
    }
}