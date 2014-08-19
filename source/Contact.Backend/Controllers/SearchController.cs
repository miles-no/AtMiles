using System.Collections.Generic;
using System.Web.Http;
using Contact.Backend.Infrastructure;
using Contact.Backend.Models.Api.Search;
using NUnit.Framework;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class SearchController : ApiController
    {
        private readonly IMediator mediator;

        public SearchController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public SearchResultModel Fulltext(string query, int? skip = 0, int? take = 10)
        {
            var request = new SearchRequestModel {Query = query, Skip = skip ?? 0, Take = take ?? 10};
            return mediator.Send<SearchRequestModel,SearchResultModel>(request, User.Identity);
        }
    }
}