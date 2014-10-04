using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class SetPrivateAddressRequest : BaseRequest
    {
        public SetPrivateAddressRequest(HttpRequestMessage request) : base(request) { }
        public string CompanyId { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string PostalName { get; set; }

    }
}