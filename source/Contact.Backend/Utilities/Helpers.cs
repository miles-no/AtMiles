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

        
        private static ConcurrentDictionary<Tuple<string,string,string>,string> cache = new ConcurrentDictionary<Tuple<string, string, string>, string>(); 
        
        public static string GetIdFromIdentity(IIdentity user, IResolveUserIdentity identityResolver)
        {
            var identity = user as ClaimsIdentity;
            if (identity != null)
            {
                var claims = identity;
                var id = claims.FindFirst(ClaimTypes.NameIdentifier);
                string globalIdentity;
                
                if (cache.TryGetValue(new Tuple<string, string, string>("miles", id.Issuer, id.Value), out globalIdentity))
                {
                    return globalIdentity;
                }

                globalIdentity = identityResolver.ResolveUserIdentityByProviderId("miles", id.Issuer, id.Value);

                if (!string.IsNullOrEmpty(globalIdentity)) return globalIdentity;

                var email = claims.FindFirst(ClaimTypes.Email);

                string message;
                globalIdentity = identityResolver.AttachLoginToUser("miles", id.Issuer, id.Value, email.Value,
                    out message);
                if (string.IsNullOrEmpty(globalIdentity))
                {
                    throw new UnknownUserException(message);
                }
                //Add to cache
                cache.AddOrUpdate(new Tuple<string, string, string>("miles", id.Issuer, id.Value), globalIdentity, (key, oldvalue)=>globalIdentity);
                return globalIdentity;
            }
            //return user.GetUserId();
            return string.Empty;
        }

    }
}