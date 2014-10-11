using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using no.miles.at.Backend.Api.Models.Api.Status;
using no.miles.at.Backend.Api.Models.Api.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Infrastructure;

namespace no.miles.at.Backend.Api.Utilities
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

        public static HttpResponseMessage CreateResponse(HttpRequestMessage request, string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = IdService.CreateNewId();
            }
            var response = new Response { RequestId = id, Status = new StatusResponse { Url = _statusEndpointUrl + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Status = message} };
            return request.CreateResponse(HttpStatusCode.Accepted, response);
        }

        public static HttpResponseMessage CreateErrorResponse(HttpRequestMessage request, string id, string errorMessage)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = IdService.CreateNewId();
            }
            var response = new Response { RequestId = id, Status = new StatusResponse { Id = "failed", Status = errorMessage } };
            return request.CreateResponse(HttpStatusCode.BadRequest, response);
        }

        public static string CreateNewId()
        {
            return IdService.CreateNewId();
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

        public static string GetUserIdentity(IIdentity user)
        {
            var identity = user as ClaimsIdentity;
            if (identity == null) return string.Empty;
            var claims = identity;
            var id = claims.FindFirst(ClaimTypes.Sid);
            if (id == null) return string.Empty;
            return id.Value;
        }

        public static bool UserHasAccessToCompany(IIdentity user, string companyId, IResolveUserIdentity identityResolver)
        {
            // This doesnt really make sense before we have more than one company
            return companyId == _companyId;
        }

        public static Person GetCreatedBy(string companyId, IIdentity user, IResolveNameOfUser nameResolver)
        {
            var userId = Helpers.GetUserIdentity(user);
            var name = nameResolver.ResolveUserNameById(companyId, userId);
            return new Person(userId, name);
        }

        public static HttpResponseMessage Send(HttpRequestMessage request, ICommandSender sender, Command command)
        {
            sender.Send(command);
            var response = Helpers.CreateResponse(request, command.CorrelationId);
            return response;
        }
    }
}