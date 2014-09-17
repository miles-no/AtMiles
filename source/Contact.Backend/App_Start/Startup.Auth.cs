using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Infrastructure;
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
                RefreshTokenProvider = new ApplicationRefreshTokenProvider(),
       
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
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(8),
                AllowInsecureHttp = true
            };

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = "atMiles",
                CookiePath = FormsAuthentication.FormsCookiePath,
                CookieSecure = CookieSecureOption.SameAsRequest,
                AuthenticationMode = AuthenticationMode.Active,
                ExpireTimeSpan = new TimeSpan(8,0,0),
                SlidingExpiration = true,
            });

            app.UseOAuthBearerTokens(OAuthOptions);
          
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Configure the application for OAuth based flow
            

            // Enable the application to use bearer tokens to authenticate users
        
            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

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

    public class ApplicationRefreshTokenProvider : IAuthenticationTokenProvider
    {
        const int Expire = 60 * 60;
          
        public void Create(AuthenticationTokenCreateContext context)
        {
            context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(Expire));
            context.SetToken(context.SerializeTicket());
        }

        public Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            return Task.Factory.StartNew(() =>
            {
                context.Ticket.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddSeconds(Expire));
                context.SetToken(context.SerializeTicket());
            });
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            context.DeserializeTicket(context.Token);
        }

        public Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            return Task.Factory.StartNew(() => context.DeserializeTicket(context.Token));
        }
    }
}
