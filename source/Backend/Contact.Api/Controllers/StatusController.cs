using System.Web.Http;
using Contact.Backend.Models.Api.Status;
using Contact.ReadStore.SessionStore;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class StatusController : ApiController
    {
        private readonly CommandStatusEngine _engine;

        public StatusController(CommandStatusEngine engine)
        {
            _engine = engine;
        }

        /// <summary>
        /// Gets the status of a request
        /// </summary>
        /// <param name="id"></param>
        [Route("api/status/{id}")]
        public StatusResponse Get(string id)
        {

            var res = _engine.GetStatus(id);
            return new StatusResponse
            {
                Id = id,
                Status = res.Status,
                ErrorMessage = res.ErrorMessage,
                Url = Request.RequestUri.AbsoluteUri
            };
        }
    }
}