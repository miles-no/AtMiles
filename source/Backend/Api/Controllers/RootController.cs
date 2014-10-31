using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace no.miles.at.Backend.Api.Controllers
{
    public class RootController : ApiController
    {
        [Route("")]
        public HttpResponseMessage Get()
        {
        	//TODO: Extend this controller to return different sections based on the user is signed in or not.
            const string res = "root";
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
