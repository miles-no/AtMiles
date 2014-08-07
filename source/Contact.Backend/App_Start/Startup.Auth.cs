using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Contact.Backend.Providers;
using Contact.Backend.Models;

namespace Contact.Backend
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //    consumerKey: "",
            //    consumerSecret: "");

            //app.UseFacebookAuthentication(
            //    appId: "",
            //    appSecret: "");
            var options = new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "387201482859-4091mlp9nvru7lfhd6mr546hku4gue2q.apps.googleusercontent.com",
                ClientSecret = "pvJiPJkQTOI6LurhaWPRfyvt",
                //Provider = new GoogleOAuth2AuthenticationProvider()
                //{
                //    OnAuthenticated = async context =>
                //    {
                //            context.Identity.AddClaim(new Claim("email", context.User.GetValue("email").ToString()));
                //        //TODO try to extact name etc

                //        //context.Identity.AddClaim(new Claim("familyname", context.FamilyName));
                //        //context.Identity.AddClaim(new Claim("givenname", context.GivenName));
                        
                // //       context.Identity.AddClaim(new Claim("F", context.User.GetValue("date-of-birth").ToString()));

                //    }
                //}
            };
            //options.Scope.Add("email");
            app.UseGoogleAuthentication(options);
        }
    }
}
