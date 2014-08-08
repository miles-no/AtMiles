using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Contact.Backend.Models.Api;
using Contact.Backend.Utilities;

namespace Contact.Backend.Controllers
{

    /// <summary>
    /// Interface for office administrators
    /// </summary>
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
        public AsyncResponseBase AddAdmin(string companyId, string officeId, [FromBody] string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse();
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
        public AsyncResponseBase RemoveAdmin(string companyId, string officeId, [FromBody] string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }

        /// <summary>
        /// Creates an employee within a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/employee")]
        public AsyncResponseBase CreateEmployee(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }


        /// <summary>
        /// Terminate an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/employee")]
        public AsyncResponseBase TerminateEmployee(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }
        
    }
}