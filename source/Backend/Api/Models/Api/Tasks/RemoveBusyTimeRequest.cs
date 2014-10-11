using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class RemoveBusyTimeRequest : BaseRequest
    {
        public RemoveBusyTimeRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
    }
}