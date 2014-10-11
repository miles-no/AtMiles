using System;
using System.Net.Http;

namespace no.miles.at.Backend.Api.Models.Api.Tasks
{
    public class SetDateOfBirthRequest : BaseRequest
    {
        public SetDateOfBirthRequest(HttpRequestMessage request) : base(request) { }
        public string CompanyId { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}