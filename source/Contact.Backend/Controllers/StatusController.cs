using System.Web.Http;
using Contact.Backend.Models.Api;
using Contact.Backend.Models.Api.StatusModels;

namespace Contact.Backend.Controllers
{

    [Authorize]
    public class StatusController : ApiController
    {
        /// <summary>
        /// Gets the status of a request
        /// </summary>
        /// <param name="id"></param>
        public Status Get(string id)
        {
            return new Status
            {
                Id = "stillPending",
                Url = Request.RequestUri.AbsoluteUri
            };
        }
         
    }
   
}