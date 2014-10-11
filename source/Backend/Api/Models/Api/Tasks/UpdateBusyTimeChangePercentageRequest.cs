using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class UpdateBusyTimeChangePercentageRequest : BaseRequest
    {
        public UpdateBusyTimeChangePercentageRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string BustTimeEntryId { get; set; }
        public short NewPercentageOccupied { get; set; }
    }
}