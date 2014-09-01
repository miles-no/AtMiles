using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contact.Domain.Services;
using Contact.Infrastructure;
using Raven.Abstractions.Data;
using Raven.Client;

namespace Contact.ReadStore.UserStore
{
    public class UserLookupEngine : IResolveUserIdentity
    {
        private readonly IDocumentStore store;

        public UserLookupEngine(IDocumentStore documentStore)
        {
            store = documentStore;
        }

        public string AttachLoginToUser(string companyid, string userId, string provider, string providerId)
        {
            var loginId = IdService.IdsToSingleLoginId(companyid, provider, providerId);

            UserLookupModel user;
            using (var session = store.OpenSession())
            {
                user = session.Load<UserLookupModel>(userId);
            }

            if (user != null && user.GlobalProviderId != loginId)
            {
                store.DatabaseCommands.Patch(user.Id, new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Set,
                        Name = "GlobalProviderId",
                        Value = loginId
                    }
                });
                return string.Empty;
            }

            //TODO: This isnt working. And it should lead to a sign out
            //List<UserLookupModel> administrators;
            //using (var session = store.OpenSession())
            //{
            //    administrators = session.Query<UserLookupModel, UserLookupIndex>().Where(w => w.CompanyId == companyid && w.AdminForOffices != null && w.AdminForOffices.Any()).ToList();
            //}

            //message = "You are not registered. Please ask an administrator to add you.";
            //if (administrators.Any())
            //{
            //    var infoBuilder = new StringBuilder();
            //    var offices = administrators.SelectMany(s => s.AdminForOffices).Distinct();
            //    foreach (var office in offices)
            //    {
            //        infoBuilder.AppendLine(office);
            //        infoBuilder.AppendLine(string.Join("\n", administrators.Where(w=>w.AdminForOffices.Any(a => a == office)).Select(s=>s.Name + "("+s.Email +")")));
            //    }
            //    message += "\nDepending where you are located you can contact one of these guys:\n" +infoBuilder;
            //}
            return string.Empty;
        }

        public string ResolveUserIdentityByProviderId(string companyId, string provider, string providerId)
        {
            var loginId = IdService.IdsToSingleLoginId(companyId, provider, providerId);
            UserLookupModel res;
            using (var session = store.OpenSession())
            {
               res = session.Query<UserLookupModel, UserLookupIndex>().FirstOrDefault(w => w.CompanyId == companyId && w.GlobalProviderId == loginId);
            }
            if (res != null)
            {
                return res.GlobalId;
            }
            return null;
        }

        public string ResolveUserIdentityByEmail(string companyId, string provider, string email)
        {
            var loginId = IdService.IdsToSingleEmailId(companyId, provider, email);
            UserLookupModel res;
            using (var session = store.OpenSession())
            {
                res = session.Query<UserLookupModel, UserLookupIndex>().FirstOrDefault(w => w.CompanyId == companyId && w.GlobalProviderEmail == loginId);
            }
            if (res != null)
            {
                return res.GlobalId;
            }
            return null;
        }
    }

}