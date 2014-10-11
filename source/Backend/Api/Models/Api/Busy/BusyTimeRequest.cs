using System.Net.Http;
using no.miles.at.Backend.Api.Models.Api.Tasks;

namespace no.miles.at.Backend.Api.Models.Api.Busy
{
    public class BusyTimeRequest : BaseRequest
    {
        public BusyTimeRequest(HttpRequestMessage request) : base(request)
        {
        }
    }
}