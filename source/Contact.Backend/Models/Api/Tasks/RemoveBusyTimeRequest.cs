using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class RemoveBusyTimeRequest : BaseRequest
    {
        public RemoveBusyTimeRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
    }
}