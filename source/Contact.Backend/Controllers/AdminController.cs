using System.Web.Http;
using Contact.Backend.Infrastructure;
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
        private readonly IMediator mediator;

        public AdminController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        /// <summary>
        /// Adds an office to a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/office")]
        public Response OpenOffice(string companyId, string officeName)
        {
            var openOfficeRequest = new OpenOfficeRequest {CompanyId = companyId, Name = officeName};
            return mediator.Send<OpenOfficeRequest, Response>(openOfficeRequest, User.Identity);
        }

        /// <summary>
        /// Closes an office in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="officeId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/office/{officeId}")]
        public Response CloseOffice(string companyId, string officeId)
        {
            var closeOfficeRequest = new CloseOfficeRequest { CompanyId = companyId, OfficeId = officeId};
            return mediator.Send<CloseOfficeRequest, Response>(closeOfficeRequest, User.Identity);
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
            var addCompanyAdminRequest = new AddCompanyAdminRequest {CompanyId = companyId, NewAdminId = employeeId};
            return mediator.Send<AddCompanyAdminRequest, Response>(addCompanyAdminRequest, User.Identity);
        }

        /// <summary>
        /// Removes administrative rights for a employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/admin/{adminId}")]
        public Response RemoveAdmin(string companyId, string adminId)
        {
            var removeCompanyAdminRequest = new RemoveCompanyAdminRequest { CompanyId = companyId, AdminId = adminId };
            return mediator.Send<RemoveCompanyAdminRequest, Response>(removeCompanyAdminRequest, User.Identity);
        
        }
        
   
    }
}