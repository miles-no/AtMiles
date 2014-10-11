using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class ImportFromCvPartnerRequest : BaseRequest
    {
        public ImportFromCvPartnerRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
    }
}