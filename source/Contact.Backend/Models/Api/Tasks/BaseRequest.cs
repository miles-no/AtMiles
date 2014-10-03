using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class BaseRequest
    {
        public BaseRequest(HttpRequestMessage request)
        {
            Request = request;
        }

        public HttpRequestMessage Request { get; private set; }
    }
}