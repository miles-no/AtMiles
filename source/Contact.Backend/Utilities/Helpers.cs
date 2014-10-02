using System;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using Contact.Backend.Models.Api.Status;
using Contact.Backend.Models.Api.Tasks;
using Contact.Infrastructure;

namespace Contact.Backend.Utilities
{
    public class Helpers
    {
        // If this system should support more then one company, we have to shift this around a bit
        private static string _companyId;
        
        private static string _statusEndpointUrl;

        public static void Initialize(string companyId, string statusEndpointUrl)
        {
            _companyId = companyId;
            _statusEndpointUrl = statusEndpointUrl;
        }

        public static Response CreateResponse(string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Domain.Services.IdService.CreateNewId();
            }
            return new Response { RequestId = id, Status = new StatusResponse { Url = _statusEndpointUrl + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Status = message} };
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

        
        private static readonly ConcurrentDictionary<Tuple<string,string>,string> Cache = new ConcurrentDictionary<Tuple<string, string>, string>();

        public static string GetUserIdentity(string userSubject, IResolveUserIdentity identityResolver)
        {
            string userId;
            if (Cache.TryGetValue(new Tuple<string, string>(_companyId, userSubject), out userId))
            {
                return userId;
            }
            userId = identityResolver.ResolveUserIdentitySubject(_companyId, userSubject);

            if (!string.IsNullOrEmpty(userId))
            {
                Cache.AddOrUpdate(new Tuple<string, string>(_companyId, userSubject), userId, (key, oldvalue) => userId);
            }
            return userId;
        }

        public static string GetUserIdentity(IIdentity user, IResolveUserIdentity identityResolver)
        {
            var identity = user as ClaimsIdentity;
            if (identity == null) return string.Empty;
            var claims = identity;
            var id = claims.FindFirst(ClaimTypes.NameIdentifier);
            if (id == null) return string.Empty;
            return id.Value;
        }

        public static bool UserHasAccessToCompany(IIdentity user, string companyId, IResolveUserIdentity identityResolver)
        {
            // This doesnt really make sense before we have more than one company
            return companyId == _companyId;
        }
    }
}