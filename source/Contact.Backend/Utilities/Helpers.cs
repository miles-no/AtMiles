using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Contact.Backend.Models.Api.Status;
using Contact.Backend.Models.Api.Tasks;
using Contact.Domain.Exceptions;
using Contact.Infrastructure;

namespace Contact.Backend.Utilities
{
    public class Helpers
    {
        private static string companyId = "miles";
        public static Response CreateResponse(string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Domain.Services.IdService.CreateNewId();
            }
            return new Response { RequestId = id, Status = new StatusResponse { Url = Config.StatusEndpoint + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Status = message} };
        }

        public static Response CreateErrorResponse(string id, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Domain.Services.IdService.CreateNewId();
            }
            return new Response { RequestId = id, Status = new StatusResponse { Id = "failed", Status = errorMessage } };
        }

        public static string CreateNewId()
        {
            return Domain.Services.IdService.CreateNewId();
        }

        
        private static readonly ConcurrentDictionary<Tuple<string,string,string>,string> Cache = new ConcurrentDictionary<Tuple<string, string, string>, string>();

        public static string GetUserIdentity(IIdentity user, IResolveUserIdentity identityResolver)
        {
            var identity = user as ClaimsIdentity;
            if (identity == null) return string.Empty;

            var claims = identity;
            var id = claims.FindFirst(ClaimTypes.NameIdentifier);

            string userId;
            if (Cache.TryGetValue(new Tuple<string, string, string>(companyId, id.Issuer, id.Value), out userId))
            {
                return userId;
            }

            userId = identityResolver.ResolveUserIdentityByProviderId(companyId, id.Issuer, id.Value);

            if (!string.IsNullOrEmpty(userId))
            {
                Cache.AddOrUpdate(new Tuple<string, string, string>(companyId, id.Issuer, id.Value), userId, (key, oldvalue) => userId);
                return userId;
            }

            //Resolve by Email
            var email = claims.FindFirst(ClaimTypes.Email);

            userId = identityResolver.ResolveUserIdentityByEmail(companyId, id.Issuer, email.Value);


            if (!string.IsNullOrEmpty(userId))
            {
                identityResolver.AttachLoginToUser(companyId, userId, id.Issuer, id.Value);
                Cache.AddOrUpdate(new Tuple<string, string, string>(companyId, id.Issuer, id.Value), userId, (key, oldvalue) => userId);
            }

            return userId;
        }

        public static void CheckIdentity(IIdentity user, IResolveUserIdentity identityResolver)
        {
            var userId = GetUserIdentity(user, identityResolver);
            
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnknownUserException("Unknown user");
            }
        }
    }
}