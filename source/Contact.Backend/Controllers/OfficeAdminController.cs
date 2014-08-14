using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Controllers
{

    /// <summary>
    /// Interface for office administrators
    /// </summary>
    [Authorize]
    public class OfficeAdminController : ApiController
    {
        private readonly IMediator mediator;

        public OfficeAdminController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Gives local administration rights to an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        //[Route("api/company/{companyId}/office/{officeId}/admin/{employeeId}")]
        [Route("api/company/{companyId}/office/{officeId}/admin")]
        public Response AddAdmin(string companyId, string officeId, [FromBody] string employeeId)
        {
            var addOfficeAdminRequest = new AddOfficeAdminRequest {CompanyId = companyId, OfficeId = officeId, AdminId = employeeId };
            return mediator.Send<AddOfficeAdminRequest, Response>(addOfficeAdminRequest, User.Identity);
       
        }

        /// <summary>
        /// Removes local administration right for an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}/admin/{adminId}")]
        public Response RemoveAdmin(string companyId, string officeId, string adminId)
        {
            var removeOfficeAdminRequest = new RemoveOfficeAdminRequest { CompanyId = companyId, OfficeId = officeId, AdminId = adminId };
            return mediator.Send<RemoveOfficeAdminRequest, Response>(removeOfficeAdminRequest, User.Identity);
  
        }

        /// <summary>
        /// Creates an employee within a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/employee")]
        public Response CreateEmployee(string companyId, AddEmployeeRequest request)
        {
            request.CompanyId = companyId;
            return mediator.Send<AddEmployeeRequest, Response>(request, User.Identity);
        }


        /// <summary>
        /// Terminate an employee
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}/employee/{employeeId}")]
        public Response TerminateEmployee(string companyId, string officeId, string employeeId)
        {
            var terminateEmployeeRequest = new TerminateEmployeeRequest { CompanyId = companyId, OfficeId = officeId, EmployeeId = employeeId};
            return mediator.Send<TerminateEmployeeRequest, Response>(terminateEmployeeRequest, User.Identity);
  
        }
        
    }
}