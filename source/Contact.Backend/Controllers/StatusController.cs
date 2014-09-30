using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Status;

namespace Contact.Backend.Controllers
{

    [Authorize]
    public class StatusController : ApiController
    {
        private readonly IMediator mediator;

        public StatusController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gets the status of a request
        /// </summary>
        /// <param name="id"></param>
        public StatusResponse Get(string id)
        {
            return mediator.Send<StatusRequest, StatusResponse>(new StatusRequest {Id = id, SenderUrl = Request.RequestUri.AbsoluteUri}, User.Identity);
        }
         
    }
   
}