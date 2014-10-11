using System;
using System.Collections.Generic;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.BusyTimeStore
{
    public class BusyTimeEngine
    {
        private readonly IDocumentStore _documentStore;

        public BusyTimeEngine(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public BusyTimeModel GetBusyTime(string employeeId)
        {
            BusyTimeModel res;
            using (var session = _documentStore.OpenSession())
            {
                var id = no.miles.at.Backend.ReadStore.BusyTimeStore.BusyTimeStore.GetRavenId(employeeId);
                res = session.Load<BusyTimeModel>(id);
            }
            if (res == null)
            {
                res = new BusyTimeModel { ExpiryDate = DateTime.UtcNow.AddDays(-1), BusyTimeEntries = new List<BusyTimeModel.BusyTime>()};
            }
            return res;
        }
    }
}
