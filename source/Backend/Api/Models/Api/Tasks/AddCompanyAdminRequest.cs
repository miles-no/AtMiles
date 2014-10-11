using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class AddCompanyAdminRequest : BaseRequest
    {
        public AddCompanyAdminRequest(HttpRequestMessage request) : base(request) { }

        public string CompanyId { get; set; }
        public string NewAdminId { get; set; }
    }
}