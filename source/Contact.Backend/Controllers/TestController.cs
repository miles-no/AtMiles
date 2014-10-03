using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class TestController : ApiController
    {
        /// <summary>
        /// Simple test that returns your name if you are authenticated. 401 if you are not
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Get()
        {
            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            var claims = authenticationManager.User.Claims.ToList();

            var sid = claims.First(c => c.Type == ClaimTypes.Sid);
            var nameId = claims.First(c => c.Type == ClaimTypes.NameIdentifier);
            var res = sid + " --- " + nameId;

            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
    }
}