using System.Web.Http;
using Contact.Backend.Models.Api;
using Contact.Backend.Utilities;

namespace Contact.Backend.Controllers
{

    /// <summary>
    /// Interface for office administrators
    /// </summary>
    [Authorize]
    public class OfficeAdminController : ApiController
    {
        /// <summary>
        /// Gives local administration rights to an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/office/{officeId}/admin")]
        public Response AddAdmin(string companyId, string officeId, [FromBody] string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }

        /// <summary>
        /// Removes local administration right for an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}/admin")]
        public Response RemoveAdmin(string companyId, string officeId, [FromBody] string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }

        /// <summary>
        /// Creates an employee within a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/employee")]
        public Response CreateEmployee(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }


        /// <summary>
        /// Terminate an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/employee")]
        public Response TerminateEmployee(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse(Request);
        }
        
    }
}