using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Microsoft.AspNet.Identity;

namespace Contact.Backend.Controllers
{
    [Authorize]
    [OverrideAuthentication]
    [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
    public class ValuesController : ApiController
    {
        // GET api/values
        //[OverrideAuthentication]
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
       // [AllowAnonymous]
        public IEnumerable<string> Get()
        {
            var ctx = Request.GetOwinContext();
var authenticationManager = ctx.Authentication;
            var t = authenticationManager.User.Claims;
            var givenName = authenticationManager.User.FindFirst(c => c.Type == ClaimTypes.GivenName);
            
            return new string[] { User.Identity.Name, "" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
