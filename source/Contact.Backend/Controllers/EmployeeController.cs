using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Busy;
using Contact.Backend.Models.Api.Employee;
using Contact.Backend.Models.Api.Tasks;
using Contact.Backend.Utilities;
using Contact.Infrastructure;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IResolveUserIdentity _identityResolver;

        public EmployeeController(IMediator mediator, IResolveUserIdentity identityResolver)
        {
            _mediator = mediator;
            _identityResolver = identityResolver;
        }

        [HttpGet]
        [Route("api/company/{companyId}/employee/{employeeId}")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage GetEmployeeDetails(string employeeId)
        {
            var res =
                _mediator.Send<EmployeeDetailsRequest, EmployeeDetailsResponse>(
                    new EmployeeDetailsRequest {EmployeeId = employeeId}, User.Identity);
            if (Helpers.UserHasAccessToCompany(User.Identity, res.CompanyId, _identityResolver) == false)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized,
                    "User does not have permission to view this employe");
            }
            return Request.CreateResponse(HttpStatusCode.OK, res);
        }

        [HttpGet]
        [Route("api/company/{companyId}/employee/busytime")]
        [ResponseType(typeof(BusyTimeResponse))]
        public BusyTimeResponse GetBusyTime(string companyId)
        {
            var request = new BusyTimeRequest(Request) {  };
            return _mediator.Send<BusyTimeRequest, BusyTimeResponse>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage AddBusyTime(string companyId, DateTime start, DateTime? end, short percentageOccupied, string comment)
        {
            var request = new AddBusyTimeRequest(Request) { CompanyId = companyId, Start = start, End = end, PercentageOccupied = percentageOccupied, Comment = comment};
            return _mediator.Send<AddBusyTimeRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpDelete]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage RemoveBusyTime(string companyId, string busyTimeId)
        {
            var request = new RemoveBusyTimeRequest(Request) { CompanyId = companyId, BustTimeEntryId = busyTimeId };
            return _mediator.Send<RemoveBusyTimeRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}/newend")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage UpdateBusyTimeNewEnd(string companyId, string busyTimeId, DateTime? newend)
        {
            var request = new UpdateBusyTimeSetEndRequest(Request) { CompanyId = companyId, BustTimeEntryId = busyTimeId, NewEnd = newend };
            return _mediator.Send<UpdateBusyTimeSetEndRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/{busyTimeId}/newpercentage")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage UpdateBusyTimeChangePercentage(string companyId, string busyTimeId, short newpercentageOccupied)
        {
            var request = new UpdateBusyTimeChangePercentageRequest(Request) { CompanyId = companyId, BustTimeEntryId = busyTimeId, NewPercentageOccupied = newpercentageOccupied};
            return _mediator.Send<UpdateBusyTimeChangePercentageRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/busytime/confirm")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage ConfirmBusyTimeEntries(string companyId)
        {
            var request = new ConfirmBusyTimeEntriesRequest(Request) { CompanyId = companyId };
            return _mediator.Send<ConfirmBusyTimeEntriesRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/setdateofbirth")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage SetPrivateAddress(string companyId, DateTime dateOfBirth)
        {
            var request = new SetDateOfBirthRequest(Request) { CompanyId = companyId, DateOfBirth = dateOfBirth};
            return _mediator.Send<SetDateOfBirthRequest, HttpResponseMessage>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/employee/setprivateaddress")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage SetPrivateAddress(string companyId, string street, string postalcode, string postalname)
        {
            var request = new SetPrivateAddressRequest(Request) { CompanyId = companyId, Street = street, PostalCode = postalcode, PostalName = postalname };
            return _mediator.Send<SetPrivateAddressRequest, HttpResponseMessage>(request, User.Identity);
        }
    }
}
