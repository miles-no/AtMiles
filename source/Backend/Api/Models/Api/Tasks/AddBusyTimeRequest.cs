using System;
using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class AddBusyTimeRequest : BaseRequest
    {
        public AddBusyTimeRequest(HttpRequestMessage request) : base(request) { }
        public string CompanyId { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public short PercentageOccupied { get; set; }
        public string Comment { get; set; }
    }
}