using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class RemoveCompanyAdminRequest : BaseRequest
    {
        public RemoveCompanyAdminRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string AdminId { get; set; }
    }
}