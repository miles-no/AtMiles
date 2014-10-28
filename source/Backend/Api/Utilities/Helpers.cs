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
    public static class Helpers
    {
        // If this system should support more then one company, we have to shift this around a bit
        private static string _companyId;
        
        private static string _statusEndpointUrl;

        public static void Initialize(string companyId, string statusEndpointUrl)
        {
            _companyId = companyId;
            _statusEndpointUrl = statusEndpointUrl;
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

        public static bool UserHasAccessToCompany(string companyId)
        {
            // This doesnt really make sense before we have more than one company
            return companyId == _companyId;
        }

        public static Person GetCreatedBy(string companyId, IIdentity user, IResolveNameOfUser nameResolver)
        {
            var userId = GetUserIdentity(user);
            var name = nameResolver.ResolveUserNameById(companyId, userId);
            return new Person(userId, name);
        }

        public static HttpResponseMessage Send(HttpRequestMessage request, ICommandSender sender, Command command)
        {
            sender.Send(command);
            var response = CreateResponse(request, command.CorrelationId);
            return response;
        }

        private static HttpResponseMessage CreateResponse(HttpRequestMessage request, string id = null, string message = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = IdService.CreateNewId();
            }
            var response = new Response { RequestId = id, Status = new StatusResponse { Url = _statusEndpointUrl + "/api/status/" + HttpUtility.UrlEncode(id), Id = "pending", Status = message } };
            return request.CreateResponse(HttpStatusCode.Accepted, response);
        }

        /// <summary>
        /// For empty images
        /// </summary>
        public const string PlaceholderImage = @"data:image/png;base64,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";
    }
}