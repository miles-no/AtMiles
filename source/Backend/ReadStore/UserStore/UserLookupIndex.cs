using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace no.miles.at.Backend.ReadStore.UserStore
{   
    public class UserLookupIndex : AbstractIndexCreationTask<UserLookupModel>
    {
        public UserLookupIndex()
        {

            Map = u =>
                from person in u
                select new
                {
                    person.GlobalProviderId,
                    person.GlobalProviderEmail,
                    person.CompanyId,
                    person.Email,
                    person.CompanyAdmin
                };

            Index(p => p.CompanyId, FieldIndexing.NotAnalyzed);
            Index(p => p.GlobalProviderId, FieldIndexing.NotAnalyzed);
            Index(p => p.GlobalProviderEmail, FieldIndexing.NotAnalyzed);
            Index(p => p.Email, FieldIndexing.NotAnalyzed);
            Index(p => p.CompanyAdmin, FieldIndexing.NotAnalyzed);
        }
    }
}