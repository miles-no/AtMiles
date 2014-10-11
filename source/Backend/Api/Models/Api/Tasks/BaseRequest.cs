using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
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