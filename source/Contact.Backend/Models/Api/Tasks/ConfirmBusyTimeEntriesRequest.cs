using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class ConfirmBusyTimeEntriesRequest : BaseRequest
    {
        public ConfirmBusyTimeEntriesRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
    }
}