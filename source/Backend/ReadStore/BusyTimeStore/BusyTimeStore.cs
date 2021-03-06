﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using no.miles.at.Backend.Domain;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Infrastructure;
using Raven.Client;

namespace no.miles.at.Backend.ReadStore.BusyTimeStore
{
    public class BusyTimeStore
    {
        private readonly IDocumentStore _documentStore;

        public BusyTimeStore(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public static string GetRavenId(string id)
        {
            return "busytime/" + id;
        }

        public void PrepareHandler(ReadModelHandler handler)
        {
            handler.RegisterHandler<EmployeeCreated>(HandleEmployeeCreated);
            handler.RegisterHandler<BusyTimeAdded>(HandleBusyTimeAdded);
            handler.RegisterHandler<BusyTimeConfirmed>(HandleBusyTimeConfirmed);
            handler.RegisterHandler<BusyTimeRemoved>(HandleBusyTimeRemoved);
            handler.RegisterHandler<BusyTimeUpdated>(HandleBusyTimeNewUpdated);
        }

        private async Task HandleEmployeeCreated(EmployeeCreated ev)
        {
            if (ev.EmployeeId == Constants.SystemUserId) return; //Do not show SYSTEM user in search

            var searchModel = ConvertTo(ev);
            using (var session = _documentStore.OpenAsyncSession())
            {
                await session.StoreAsync(searchModel);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeAdded(BusyTimeAdded ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<BusyTimeModel>(GetRavenId(ev.EmployeeId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeNewUpdated(BusyTimeUpdated ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<BusyTimeModel>(GetRavenId(ev.EmployeeId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeRemoved(BusyTimeRemoved ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<BusyTimeModel>(GetRavenId(ev.EmployeeId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private async Task HandleBusyTimeConfirmed(BusyTimeConfirmed ev)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var model = await session.LoadAsync<BusyTimeModel>(GetRavenId(ev.EmployeeId));
                model = Patch(model, ev);
                await session.StoreAsync(model);
                await session.SaveChangesAsync();
            }
        }

        private static BusyTimeModel Patch(BusyTimeModel model, BusyTimeRemoved ev)
        {
            if (model.BusyTimeEntries != null)
            {
                model.BusyTimeEntries.RemoveAll(b => b.Id == ev.BusyTimeId);
            }
            return UpdateExpiryDate(model, ev.Created);
        }

        private BusyTimeModel Patch(BusyTimeModel model, BusyTimeUpdated ev)
        {
            if (model.BusyTimeEntries != null)
            {
                if (model.BusyTimeEntries.Any(bt => bt.Id == ev.BusyTimeId))
                {
                    var busy = model.BusyTimeEntries.First(bt => bt.Id == ev.BusyTimeId);
                    busy.Start = ev.Start;
                    busy.End = ev.End;
                    busy.PercentageOccupied = ev.PercentageOccpied;
                    busy.Comment = ev.Comment;
                }
            }
            return UpdateExpiryDate(model, ev.Created);
        }

        private static BusyTimeModel Patch(BusyTimeModel model, BusyTimeConfirmed ev)
        {
            return UpdateExpiryDate(model, ev.Created);
        }

        private static BusyTimeModel Patch(BusyTimeModel model, BusyTimeAdded ev)
        {
            if (model.BusyTimeEntries == null) model.BusyTimeEntries = new List<BusyTimeModel.BusyTime>();
            model.BusyTimeEntries.Add(new BusyTimeModel.BusyTime { Id = ev.BusyTimeId, Start = ev.Start, End = ev.End, PercentageOccupied = ev.PercentageOccpied, Comment = ev.Comment });
            return UpdateExpiryDate(model, ev.Created);
        }

        private static BusyTimeModel ConvertTo(EmployeeCreated ev)
        {
            var model = new BusyTimeModel
            {
                Id = GetRavenId(ev.EmployeeId),
                ExpiryDate = DateTime.UtcNow.AddDays(-1),
                BusyTimeEntries = new List<BusyTimeModel.BusyTime>()
            };
            return model;
        }

        private static BusyTimeModel UpdateExpiryDate(BusyTimeModel model, DateTime created)
        {
            model.ExpiryDate = created.AddDays(90);
            return model;
        }
    }
}
