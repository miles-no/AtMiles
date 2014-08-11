using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Results;
using Contact.Backend.Models.Api;

namespace Contact.Backend.Utilities
{
    public class Config
    {
        public static readonly string StatusEndpoint = ConfigurationManager.AppSettings["statusEndpoint"];
    }

    public class Helpers
    {
        public static string CreateNewId()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray().Take(14).ToArray());
        }

        public static Response CreateResponse(string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = CreateNewId();
            }
            return new Response { RequestId = id, Status = new Status { Url = Config.StatusEndpoint + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Message = message} };
        }

        public static Response CreateErrorResponse(string id, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = CreateNewId();
            }
            return new Response { RequestId = id, Status = new Status { Id = "failed", Message = errorMessage } };
        }
    }
}