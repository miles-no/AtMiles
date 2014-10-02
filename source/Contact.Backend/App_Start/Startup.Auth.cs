﻿using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Contact.Infrastructure.Configuration;
using Microsoft.Owin.Cors;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using Owin;
using Microsoft.Owin.Security.Jwt;
using System.Web.Http;
using Microsoft.Owin.Security.DataHandler.Encoder;
using Microsoft.Owin.Security;

namespace Contact.Backend
{
    public partial class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void ConfigureAuth(IAppBuilder app, Config settings)
        {
            var issuer = settings.Auth0Issuer;
            var audience = settings.Auth0Audience;
            var secret = TextEncodings.Base64Url.Decode(settings.Auth0Secret);

            string token = string.Empty;

            // Api controllers with an [Authorize] attribute will be validated with JWT
            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = AuthenticationMode.Active,
                    AllowedAudiences = new[] { audience },
                    IssuerSecurityTokenProviders = new IIssuerSecurityTokenProvider[]
                    {
                        new SymmetricKeyIssuerSecurityTokenProvider(issuer, secret)
                    },
                    Provider = new OAuthBearerAuthenticationProvider
                    {
                        OnRequestToken = context =>
                        {
                            token = context.Token;
                            return Task.FromResult<object>(null);
                        },
                        OnValidateIdentity = context =>
                        {
                            if (!string.IsNullOrEmpty(token))
                            {
                                var notPadded = token.Split('.')[1];
                                var claimsPart = Convert.FromBase64String(
                                    notPadded.PadRight(notPadded.Length + (4 - notPadded.Length%4)%4, '='));

                                var obj = JObject.Parse(Encoding.UTF8.GetString(claimsPart, 0, claimsPart.Length));

                                // simple, not handling specific types, arrays, etc.
                                foreach (var prop in obj.Properties().AsJEnumerable())
                                {
                                    if (!context.Ticket.Identity.HasClaim(prop.Name, prop.Value.Value<string>()))
                                    {
                                        context.Ticket.Identity.AddClaim(new Claim(prop.Name, prop.Value.Value<string>()));
                                    }
                                }
                                //TODO: Lookup and add userId here. If not existing: Add new user
                                //TODO: Check if user has been terminated, and reject here if so
                            }
                            else
                            {
                                context.Rejected();
                            }
                            return Task.FromResult<object>(null);
                        }
                    }
                });

            app.UseCors(CorsOptions.AllowAll);

            // Only use external cookie for authentication
            var config = GlobalConfiguration.Configuration;
            config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(DefaultAuthenticationTypes.ExternalCookie));
        }
    }
}
