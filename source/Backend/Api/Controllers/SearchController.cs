using System.Collections.Generic;
using System.Web.Http;
using no.miles.at.Backend.Api.Models.Api.Search;
using no.miles.at.Backend.Api.Utilities;
using no.miles.at.Backend.ReadStore.SearchStore;

namespace no.miles.at.Backend.Api.Controllers
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
            int takeNumber = take ?? 10;
            int skipNumber = skip ?? 0;

            int total;
            var resSearch = _engine.FulltextSearch(query, takeNumber, skipNumber, out total);

            var res = new SearchResultModel {Skipped = skipNumber, Total = total, Results = Convert(resSearch)};

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
                        Thumb = (entry.Thumb == null || entry.Thumb.Length < 100) ? Helpers.PlaceholderImage : entry.Thumb,
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