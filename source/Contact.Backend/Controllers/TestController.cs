using System.Linq;
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
        public string Get()
        {
            string res = "Welcome {0}";

            var ctx = Request.GetOwinContext();
            var authenticationManager = ctx.Authentication;
            var claims = authenticationManager.User.Claims.ToList();
            
            var emailClaim = claims.First(c => c.Type == ClaimTypes.Email);
 
            var givenNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);

            if (givenNameClaim == null)
            {
                res = string.Format(res, emailClaim.Value, string.Empty, string.Empty);
            }
            else
            {
                res = string.Format(res, givenNameClaim.Value + " (" + emailClaim.Value + ")");
            }


            return res;
        }

    }
}