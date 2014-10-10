using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class RemoveCompanyAdminRequest : BaseRequest
    {
        public RemoveCompanyAdminRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string AdminId { get; set; }
    }
}