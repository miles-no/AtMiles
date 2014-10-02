using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Tasks;

namespace Contact.Backend.Controllers
{
    /// <summary>
    /// Administrative interface
    /// </summary>
    [Authorize]
    public class AdminController : ApiController
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [HttpPost]
        [Route("api/company/{companyId}/importCvPartner")]
        public Response ImportFromCvPartner(string companyId)
        {
            var importRequest = new ImportFromCvPartnerRequest { CompanyId = companyId };
            return _mediator.Send<ImportFromCvPartnerRequest, Response>(importRequest, User.Identity);
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
            return _mediator.Send<AddCompanyAdminRequest, Response>(addCompanyAdminRequest, User.Identity);
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
            return _mediator.Send<RemoveCompanyAdminRequest, Response>(removeCompanyAdminRequest, User.Identity);
        
        }
    }
}