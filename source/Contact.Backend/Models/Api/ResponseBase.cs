using Contact.Backend.Controllers;

namespace Contact.Backend.Models.Api
{
    public class ResponseBase
    {
        public string RequestId { get; set; }
        public Status Status { get; set; }
    }
}