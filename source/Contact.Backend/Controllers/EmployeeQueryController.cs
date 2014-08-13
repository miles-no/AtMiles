using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Contact.Backend.Models.Api.Queries;
using NUnit.Framework;

namespace Contact.Backend.Controllers
{
    public class EmployeeQueryController : ApiController
    {

        [HttpGet]
        [Route("api/employee/search")]
        public Task<List<EmployeeFulltextSearchResponse>> FulltextSearch(string searchString)
        {
            //TODO
            return null;
            
        }
    }

    public class OfficeQueryController : ApiController
    {


    }
}