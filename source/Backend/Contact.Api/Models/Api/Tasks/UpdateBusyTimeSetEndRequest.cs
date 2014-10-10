using System;
using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class UpdateBusyTimeSetEndRequest : BaseRequest
    {
        public UpdateBusyTimeSetEndRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
        public DateTime? NewEnd { get; set; }
    }
}