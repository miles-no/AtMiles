using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contact.Backend.MockStore;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using Contact.Infrastructure;
using Contact.ReadStore.Test.SearchStore;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Indexes;

namespace Contact.ReadStore.Test.UserStore
{
    public class UserLookupEngine : IResolveUserIdentity
    {
        private readonly IDocumentStore store = MockStore.DocumentStore;

        public UserLookupEngine()
        {
       
        }

        public string AttachLoginToUser(string companyid, string provider, string providerId, string email, out string message)
        {
            
            
            // Not necessary?
            //if (login.Email.Contains("@" + companyid) == false)
            //{
            //    message = "Email address is not valid for this company";
            //    return false;
            //}

            var globalId = IdService.IdsToSingle(companyid, provider, providerId);

            

            User user;
            using (var session = store.OpenSession())
            {
                user = session.Query<User, UserLookupIndex>().FirstOrDefault(w => w.CompanyId == companyid && w.Email == email);
            }
            
            if (user != null)
            {
                store.DatabaseCommands.Patch(user.Id, new[]
                {
                    new PatchRequest
                    {
                        Type = PatchCommandType.Set,
                        Name = "GlobalId",
                        Value = globalId
                    }
                });
                message = "Goodie";
                return globalId;
            }

            //TODO: This isnt working. And it should lead to a sign out
            List<User> administrators;
            using (var session = store.OpenSession())
            {
                administrators = session.Query<User, UserLookupIndex>().Where(w => w.CompanyId == companyid && w.AdminForOffices != null && w.AdminForOffices.Any()).ToList();
            }

            message = "You are not registered. Please ask an administrator to add you.";
            if (administrators.Any())
            {
                var infoBuilder = new StringBuilder();
                var offices = administrators.SelectMany(s => s.AdminForOffices).Distinct();
                foreach (var office in offices)
                {
                    infoBuilder.AppendLine(office);
                    infoBuilder.AppendLine(string.Join("\n", administrators.Where(w=>w.AdminForOffices.Any(a => a == office)).Select(s=>s.Name + "("+s.Email +")")));
                }
                message += "\nDepending where you are located you can contact one of these guys:\n" +infoBuilder;
            }

            return null;

        }

        public string ResolveUserIdentityByProviderId(string companyId, string provider, string providerId)
        {
            var fullId = IdService.IdsToSingle(companyId, provider, providerId);
            bool res;
            using (var session = store.OpenSession())
            {
               res = session.Query<User, UserLookupIndex>().Any(w => w.GlobalId == fullId);
            }
            if (res)
            {
                return fullId;
            }
            return null;
        }

        public string ResolveUserIdentityByEmail(string companyId, string provider, string email)
        {
            throw new NotImplementedException();
        }
    }

}