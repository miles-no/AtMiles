using System.Linq;
using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore.UserStore
{
    public class UserLookupEngine : IResolveUserIdentity
    {
        private readonly IDocumentStore _store;

        public UserLookupEngine(IDocumentStore documentStore)
        {
            _store = documentStore;
        }

        public string ResolveUserIdentitySubject(string companyId, string subject)
        {
            if (subject == null) return string.Empty;
            var parts = subject.Split('|');
            if (parts.Length != 2) return string.Empty;
            string provider = parts[0];
            var email = parts[1];

            UserLookupModel res;
            using (var session = _store.OpenSession())
            {
                res = session.Query<UserLookupModel, UserLookupIndex>().FirstOrDefault(w => w.CompanyId == companyId && w.GlobalProviderId == provider && w.GlobalProviderEmail == email);
            }
            if (res != null)
            {
                return res.GlobalId;
            }
            return null;
        }
    }
}