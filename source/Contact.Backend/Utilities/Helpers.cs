using System.Web;
using Contact.Backend.Models.Api.Status;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Utilities
{
    public class Helpers
    {
        public static Response CreateResponse(string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Domain.Services.IdService.CreateNewId();
            }
            return new Response { RequestId = id, Status = new StatusResponse { Url = Config.StatusEndpoint + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Status = message} };
        }

        public static Response CreateErrorResponse(string id, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Domain.Services.IdService.CreateNewId();
            }
            return new Response { RequestId = id, Status = new StatusResponse { Id = "failed", Status = errorMessage } };
        }

        public static string CreateNewId()
        {
            return Domain.Services.IdService.CreateNewId();
        }
    }
}