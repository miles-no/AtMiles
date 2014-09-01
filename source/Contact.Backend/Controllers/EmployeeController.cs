using System;
using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class EmployeeController : ApiController
    {
        private readonly IMediator mediator;

        public EmployeeController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        [Route("api/company/{companyId}/office({officeId}/employee/busytime")]
        public Response AddBusyTime(string companyId, string officeId, DateTime start, DateTime? end, short percentageOccupied, string comment)
        {
            var request = new AddBusyTimeRequest { CompanyId = companyId, OfficeId = officeId, Start = start, End = end, PercentageOccupied = percentageOccupied, Comment = comment};
            return mediator.Send<AddBusyTimeRequest, Response>(request, User.Identity);
        }

        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}/employee/busytime/{busyTimeId}")]
        public Response RemoveBusyTime(string companyId, string officeId, string busyTimeId)
        {
            var request = new RemoveBusyTimeRequest { CompanyId = companyId, OfficeId = officeId, BustTimeEntryId = busyTimeId };
            return mediator.Send<RemoveBusyTimeRequest, Response>(request, User.Identity);
        }

        [HttpPost]
        [Route("api/company/{companyId}/office/{officeId}/employee/busytime/confirm")]
        public Response ConfirmBusyTimeEntries(string companyId, string officeId)
        {
            var request = new ConfirmBusyTimeEntriesRequest { CompanyId = companyId, OfficeId = officeId };
            return mediator.Send<ConfirmBusyTimeEntriesRequest, Response>(request, User.Identity);
        }

    }
}
