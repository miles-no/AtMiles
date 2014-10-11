using System;
using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class UpdateBusyTimeSetEndRequest : BaseRequest
    {
        public UpdateBusyTimeSetEndRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
        public DateTime? NewEnd { get; set; }
    }
}