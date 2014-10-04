﻿using System;
using System.Linq;
using Contact.Domain.Services;
using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore.UserStore
{
    public class UserLookupEngine : IResolveUserIdentity, IResolveNameOfUser
    {
        private readonly IDocumentStore _store;

        public UserLookupEngine(IDocumentStore documentStore)
        {
            _store = documentStore;
        }

        public string ResolveUserIdentitySubject(string companyId, string subject)
        {
            var splitted = IdService.SplitLoginSubject(subject);
            var provider = splitted.Item1;
            var email = splitted.Item2;

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

        public string ResolveUserNameById(string companyId, string userId)
        {
            UserLookupModel res;
            using (var session = _store.OpenSession())
            {
                res = session.Load<UserLookupModel>("users/" + userId);
            }
            if (res != null)
            {
                return res.Name;
            }
            return string.Empty;
        }
    }
}