using System.Net.Http;
using no.miles.at.Backend.Api.Models.Api.Tasks;

namespace no.miles.at.Backend.Api.Models.Api.Admins
{
    public class GetCompanyAdminsRequest : BaseRequest
    {
        public string CompanyId { get; set; }
        public GetCompanyAdminsRequest(HttpRequestMessage request) : base(request)
        {
        }
    }
}