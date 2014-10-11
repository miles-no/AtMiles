using System.Collections.Generic;
using System.Web.Http;
using Contact.Backend.Models.Api.Search;
using Contact.ReadStore.SearchStore;

namespace Contact.Backend.Controllers
{
    [Authorize]
    public class SearchController : ApiController
    {
        private readonly EmployeeSearchEngine _engine;

        public SearchController(EmployeeSearchEngine engine)
        {
            _engine = engine;
        }

        [HttpGet]
        [Route("api/search/Fulltext")]
        public SearchResultModel Fulltext(string query, int? skip = 0, int? take = 10)
        {
            var request = new SearchRequestModel {Query = query, Skip = skip ?? 0, Take = take ?? 10};

            int takeNumber = take ?? 10;
            int skipNumber = skip ?? 0;

            int total;
            var resSearch = _engine.FulltextSearch(query, takeNumber, skipNumber, out total);

            var res = new SearchResultModel { Skipped = skipNumber, Total = total };
            res.Results = Convert(resSearch);

            return res;

        }

        private static List<Result> Convert(IEnumerable<EmployeeSearchModel> i)
        {
            var result = new List<Result>();
            if (i != null)
            {
                foreach (var entry in i)
                {
                    var convertedEntry = new Result
                    {
                        CompanyId = entry.CompanyId,
                        OfficeName = entry.OfficeName,
                        GlobalId = entry.GlobalId,
                        Name = entry.Name,
                        DateOfBirth = entry.DateOfBirth,
                        JobTitle = entry.JobTitle,
                        PhoneNumber = entry.PhoneNumber,
                        Email = entry.Email,
                        Thumb = entry.Thumb,
                        PrivateAddress = new Result.Address()
                    };
                    if (entry.PrivateAddress != null)
                    {
                        convertedEntry.PrivateAddress.Street = entry.PrivateAddress.Street;
                        convertedEntry.PrivateAddress.PostalCode = entry.PrivateAddress.PostalCode;
                        convertedEntry.PrivateAddress.PostalName = entry.PrivateAddress.PostalName;
                    }
                    result.Add(convertedEntry);
                }
            }
            return result;
        }
    }
}