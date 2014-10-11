using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class ConfirmBusyTimeEntriesRequest : BaseRequest
    {
        public ConfirmBusyTimeEntriesRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
    }
}