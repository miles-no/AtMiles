using System.Web.Http;
using Contact.Backend.Models.Api;
using Contact.Backend.Utilities;

namespace Contact.Backend.Controllers
{
    /// <summary>
    /// Administrative interface
    /// </summary>
    public class AdminController : ApiController
    {
        /// <summary>
        /// Adds an office to a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/office")]
        public AsyncResponseBase AddOffice(string companyId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }

        /// <summary>
        /// Removes an office in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}")]
        public AsyncResponseBase RemoveOffice(string companyId, string officeId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }

        /// <summary>
        /// Give administrator rights to an employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/admin/{employeeId}")]
        public AsyncResponseBase AddAdmin(string companyId, string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }

        /// <summary>
        /// Removes administrative rights for a employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/admin/{employeeId}")]
        public AsyncResponseBase RemoveAdmin(string companyId, string employeeId)
        {
            return ControllerHelpers.CreateDummyResponse();
        }
        
   
    }
}