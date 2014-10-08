using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Admins;
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
        [ResponseType(typeof(Response))]
        public HttpResponseMessage ImportFromCvPartner(string companyId)
        {
            //HttpResponseMessage 
            var importRequest = new ImportFromCvPartnerRequest(Request) { CompanyId = companyId };
            return _mediator.Send<ImportFromCvPartnerRequest, HttpResponseMessage>(importRequest, User.Identity);
            
        }

        [HttpGet]
        [Route("api/company/{companyId}/admin")]
        [ResponseType(typeof(Response))]
        public GetCompanyAdminsResponse GetAllAdmins(string companyId)
        {
            var getCompanyAdmins = new GetCompanyAdminsRequest(Request){CompanyId = companyId};
            return _mediator.Send<GetCompanyAdminsRequest, GetCompanyAdminsResponse>(getCompanyAdmins, User.Identity);
        }

        /// <summary>
        /// Give administrator rights to an employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/company/{companyId}/admin/{employeeId}")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage AddAdmin(string companyId, string employeeId)
        {
            var addCompanyAdminRequest = new AddCompanyAdminRequest(Request) { CompanyId = companyId, NewAdminId = employeeId };
            return _mediator.Send<AddCompanyAdminRequest, HttpResponseMessage>(addCompanyAdminRequest, User.Identity);
        }

        /// <summary>
        /// Removes administrative rights for a employee in a company
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="adminId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/company/{companyId}/admin/{adminId}")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage RemoveAdmin(string companyId, string adminId)
        {
            var removeCompanyAdminRequest = new RemoveCompanyAdminRequest(Request) { CompanyId = companyId, AdminId = adminId };
            return _mediator.Send<RemoveCompanyAdminRequest, HttpResponseMessage>(removeCompanyAdminRequest, User.Identity);
        }
    }
}