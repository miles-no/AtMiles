using System;
using System.Net.Http;

namespace Contact.Backend.Models.Api.Tasks
{
    public class SetDateOfBirthRequest : BaseRequest
    {
        public SetDateOfBirthRequest(HttpRequestMessage request) : base(request) { }
        public string CompanyId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}