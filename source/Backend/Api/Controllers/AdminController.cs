using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using no.miles.at.Backend.Api.Models.Api.Admins;
using no.miles.at.Backend.Api.Models.Api.Tasks;
using no.miles.at.Backend.Api.Utilities;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Infrastructure;
using no.miles.at.Backend.ReadStore.UserStore;

namespace no.miles.at.Backend.Api.Controllers
{
    /// <summary>
    /// Administrative interface
    /// </summary>
    [Authorize]
    public class AdminController : ApiController
    {
        private readonly IResolveNameOfUser _nameResolver;
        private readonly ICommandSender _commandSender;
        private readonly UserLookupEngine _engine;

        public AdminController(IResolveNameOfUser nameResolver, ICommandSender commandSender, UserLookupEngine engine)
        {
            _nameResolver = nameResolver;
            _commandSender = commandSender;
            _engine = engine;
        }

        [HttpPost]
        [Route("api/company/{companyId}/importCvPartner")]
        [ResponseType(typeof(Response))]
        public HttpResponseMessage ImportFromCvPartner(string companyId)
        {
            string correlationId = Helpers.CreateNewId();
            try
            {
                var command = new ImportDataFromCvPartner(companyId, DateTime.UtcNow, Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver), correlationId, Constants.IgnoreVersion);

                return Helpers.Send(Request, _commandSender, command);

            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
            
        }

        [HttpGet]
        [Route("api/company/{companyId}/admin")]
        [ResponseType(typeof(Response))]
        public GetCompanyAdminsResponse GetAllAdmins(string companyId)
        {
            var data = _engine.GetAllCompanyAdmins(companyId);
            return Convert(data);
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
            string correlationId = Helpers.CreateNewId();

            try
            {
                var command = new AddCompanyAdmin(companyId, employeeId, DateTime.UtcNow, Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver), correlationId, Constants.IgnoreVersion);

                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
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
            string correlationId = Helpers.CreateNewId();

            try
            {
                var command = new RemoveCompanyAdmin(companyId, adminId, DateTime.UtcNow, Helpers.GetCreatedBy(companyId, User.Identity, _nameResolver), correlationId, Constants.IgnoreVersion);

                return Helpers.Send(Request, _commandSender, command);
            }
            catch (Exception ex)
            {
                return Helpers.CreateErrorResponse(Request, correlationId, ex.Message);
            }
        }

        private static GetCompanyAdminsResponse Convert(IEnumerable<UserLookupModel> i)
        {
            var response = new GetCompanyAdminsResponse { Admins = new List<GetCompanyAdminsResponse.Admin>() };
            if (i != null)
            {
                foreach (var userLookupModel in i)
                {
                    response.Admins.Add(new GetCompanyAdminsResponse.Admin { Id = userLookupModel.GlobalId, Name = userLookupModel.Name });
                }
            }
            return response;
        }
    }
}