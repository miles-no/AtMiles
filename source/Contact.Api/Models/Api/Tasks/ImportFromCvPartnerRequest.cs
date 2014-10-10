using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class ImportFromCvPartnerRequest : BaseRequest
    {
        public ImportFromCvPartnerRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
    }
}