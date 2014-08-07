using System;
using System.Linq;
using System.Web;
using Contact.Backend.Models.Api;

namespace Contact.Backend.Utilities
{
    public class ControllerHelpers
    {
        public static string CreateNewId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray().Take(14).ToArray());
        }

        public static AsyncResponseBase CreateDummyResponse()
        {
            var id = CreateNewId();
            return new AsyncResponseBase {Id = id, Status = "pending", Url = "http://some_url/" + HttpUtility.UrlEncode(id)};
        }
    }
}