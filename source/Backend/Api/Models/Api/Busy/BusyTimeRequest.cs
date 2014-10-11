using System.Net.Http;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Models.Api.Busy
{
    public class BusyTimeRequest : BaseRequest
    {
        public BusyTimeRequest(HttpRequestMessage request) : base(request)
        {
        }
    }
}