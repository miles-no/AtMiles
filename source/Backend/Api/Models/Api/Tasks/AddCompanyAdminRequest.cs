using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class AddCompanyAdminRequest : BaseRequest
    {
        public AddCompanyAdminRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string NewAdminId { get; set; }
    }
}