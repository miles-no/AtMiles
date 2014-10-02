using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Contact.Backend.Utilities;
using Contact.Domain.Exceptions;
using Contact.Infrastructure;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Contact.Backend.Results;

namespace Contact.Backend.Controllers
{
    /// <summary>
    /// Web Api template interface for working OAUTH2/google magic
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private readonly IResolveUserIdentity resolveUserIdentity;

        // POST api/Account/Logout
        public AccountController(IResolveUserIdentity resolveUserIdentity)
        {
            this.resolveUserIdentity = resolveUserIdentity;
        }


        // GET api/Account/ExternalLogin
        /// <summary>
        /// Logs in the user using an external provider. Supported provider is as of now Google. Use url result of api/account/externalogins for correct parameters
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="error"></param>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null, string redirect_uri = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }


            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            try
            {
                Helpers.CheckIdentity(User.Identity, resolveUserIdentity);
            }
            catch (UnknownUserException ex)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                
                //TODO serve a proper message for the user
                Debug.WriteLine(ex.Message);
               
            }

            if (redirect_uri != null)
            {
                return Redirect(redirect_uri);
            }
            return Ok();
        }

      
        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
         
            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }
              
                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                };
            }
        }
        #endregion
    }
}
