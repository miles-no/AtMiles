using System;
using System.Linq;
using System.Net.Http;
using System.Web;
using Contact.Backend.Models.Api;
using Contact.Backend.Models.Api.StatusModels;

namespace Contact.Backend.Utilities
{
    public class ControllerHelpers
    {
        public static string CreateNewId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray().Take(14).ToArray());
        }

        public static Response CreateDummyResponse(HttpRequestMessage request)
        {
            var uri = request.RequestUri;
            var host = uri.GetLeftPart(UriPartial.Authority);
            var id = CreateNewId();
            return new Response {RequestId = id, Status = new Status{Url = host + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending"}};
        }
    }
}