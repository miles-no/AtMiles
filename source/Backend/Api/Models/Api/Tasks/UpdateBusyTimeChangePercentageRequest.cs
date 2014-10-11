using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class UpdateBusyTimeChangePercentageRequest : BaseRequest
    {
        public UpdateBusyTimeChangePercentageRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
        public short NewPercentageOccupied { get; set; }
    }
}