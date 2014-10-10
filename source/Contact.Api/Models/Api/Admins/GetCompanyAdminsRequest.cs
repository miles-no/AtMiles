using System.Net.Http;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Models.Api.Admins
{
    public class GetCompanyAdminsRequest : BaseRequest
    {
        public string CompanyId { get; set; }
        public GetCompanyAdminsRequest(HttpRequestMessage request) : base(request)
        {
        }
    }
}