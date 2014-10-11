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
            var res = "root";
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}
