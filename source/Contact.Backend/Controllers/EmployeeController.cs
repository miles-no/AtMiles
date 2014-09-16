using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Employee;
using Contact.Backend.Models.Api.Tasks;
using Contact.Backend.Utilities;
using Contact.Infrastructure;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly IMediator mediator;
        private readonly IResolveUserIdentity identityResolver;

        public EmployeeController(IMediator mediator, IResolveUserIdentity identityResolver)
        {
            this.mediator = mediator;
            this.identityResolver = identityResolver;
        }

        [HttpGet]
        [Route("api/company/{companyId}/employee/{employeeId}")]
        public HttpResponseMessage GetEmployeeDetails(string employeeId)
        {
            var res =
                mediator.Send<EmployeeDetailsRequest, EmployeeDetailsResponse>(
                    new EmployeeDetailsRequest {EmployeeId = employeeId}, User.Identity);
            if (Helpers.UserHasAccessToCompany(User.Identity, res.CompanyId, identityResolver) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User does not have permission to view this employe");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }
        

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime")]
        public Response AddBusyTime(string companyId, DateTime start, DateTime? end, short percentageOccupied, string comment)
        {
            var request = new AddBusyTimeRequest { CompanyId = companyId, Start = start, End = end, PercentageOccupied = percentageOccupied, Comment = comment};
            return mediator.Send<AddBusyTimeRequest, Response>(request, User.Identity);
        }

        [HttpDelete]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}")]
        public Response RemoveBusyTime(string companyId, string busyTimeId)
        {
            var request = new RemoveBusyTimeRequest { CompanyId = companyId, BustTimeEntryId = busyTimeId };
            return mediator.Send<RemoveBusyTimeRequest, Response>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/confirm")]
        public Response ConfirmBusyTimeEntries(string companyId)
        {
            var request = new ConfirmBusyTimeEntriesRequest { CompanyId = companyId };
            return mediator.Send<ConfirmBusyTimeEntriesRequest, Response>(request, User.Identity);
        }

    }
}
