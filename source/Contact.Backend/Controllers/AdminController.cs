using System.Web.Http;
using Contact.Backend.Models.Api;
using Contact.Backend.Utilities;

namespace Contact.Backend.Controllers
{
    /// <summary>
    /// Administrative interface
    /// </summary>
    [Authorize]
    public class AdminController : ApiController
    {
        /// <summary>
        /// Adds an office to a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/office")]
        public Response AddOffice(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }

        /// <summary>
        /// Removes an office in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}")]
        public Response RemoveOffice(string companyId, string officeId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }

        /// <summary>
        /// Give administrator rights to an employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/admin/{employeeId}")]
        public Response AddAdmin(string companyId, string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }

        /// <summary>
        /// Removes administrative rights for a employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/admin/{employeeId}")]
        public Response RemoveAdmin(string companyId, string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }
        
   
    }
}