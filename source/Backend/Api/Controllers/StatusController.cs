using System.Web.Http;
using no.miles.at.Backend.Api.Models.Api.Status;
using no.miles.at.Backend.ReadStore.SessionStore;

namespace no.miles.at.Backend.Api.Controllers
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
            if (res == null || res.Id != id)
            {
                //TODO: Log and return
                return null;
            }
            return new StatusResponse
            {
                Id = res.Id,
                CommandName = res.CommandName,
                Status = res.Status,
                ErrorMessage = res.ErrorMessage,
                Url = Request.RequestUri.AbsoluteUri,
                Started = res.Started,
                Finished = res.Finished
            };
        }
    }
}