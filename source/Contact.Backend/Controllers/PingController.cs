using System.Web.Http;

namespace Contact.Backend.Controllers
{
    public class PingController : ApiController
    {
        [HttpGet]
        [Route("api/ping")]
        public IHttpActionResult NotSecured()
        {
            return this.Ok("All good. You don't need to be authenticated to call this.");
        }

        [Authorize]
        [HttpGet]
        [Route("api/secured/ping")]
        public IHttpActionResult Secured()
        {
            return this.Ok("All good. You only get this message if you are authenticated.");
        }
    }
}
