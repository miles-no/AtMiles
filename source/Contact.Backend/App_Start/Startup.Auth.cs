using System;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Contact.Backend
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            PublicClientId = "self";
    
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"), 
       
                //TODO: Add additional validation if necessary
                Provider = new OAuthAuthorizationServerProvider
                {
                     OnValidateClientRedirectUri = async (context) =>
                     {
                         context.Validated(context.RedirectUri);
                     },
                     OnValidateClientAuthentication = async (context) =>
                     {
                         context.Validated(context.ClientId);
                     },
                     OnGrantResourceOwnerCredentials = async (context) =>
                     {
                         context.Validated(context.Ticket);
                     },
                     OnGrantClientCredentials = async (context) =>
                     {
                         context.Validated(context.Ticket);
                     }
                },
             
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(1),
                AllowInsecureHttp = true
            };

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOAuthBearerTokens(OAuthOptions);
          
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            

            // Enable the application to use bearer tokens to authenticate users
        
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
            };

            app.UseGoogleAuthentication(options);

            // Only use external cookie for authentication
            var config = GlobalConfiguration.Configuration;
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(DefaultAuthenticationTypes.ExternalCookie));

          
        }
    }
}
