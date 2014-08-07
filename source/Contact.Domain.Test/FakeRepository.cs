using System;
using System.Collections.Generic;
using Contact.Domain.Exceptions;

namespace Contact.Domain.Test
{
    public class FakeRepository<T> : IRepository<T> where T : AggregateRoot, new()
    {
        private List<FakeStreamEvent> GivenEvents { get; set; }
        private List<Event> ThenEvents { get; set; }

        public FakeRepository(IEnumerable<FakeStreamEvent> givens)
        {
            GivenEvents = new List<FakeStreamEvent>(givens);
            ThenEvents = new List<Event>();
        }

        public List<Event> GetThenEvents()
        {
            return ThenEvents;
        }

        public void Save(T aggregate, int expectedVersion)
        {
            var eventsToSave = aggregate.GetUncommittedChanges();
            ThenEvents.AddRange(eventsToSave);
            aggregate.MarkChangesAsCommitted();
        }

        public T GetById(string id)
        {
            return GetById(id, false);
        }

        public T GetById(string id, bool keepHistory)
        {
            if (id == string.Empty)
            {
                throw new UnknownItemException();
            }

            T aggregate = new T();

            aggregate.LoadsFromHistory(GetEventsById(id),keepHistory);

            if (aggregate.Id != id)
            {
                throw new UnknownItemException();
            }
            return aggregate;
        }

        private List<Event> GetEventsById(string id)
        {
            var events = new List<Event>();
            if (GivenEvents != null)
            {
                foreach (var givenEvent in GivenEvents)
                {
                    if (givenEvent.StreamId == id)
                    {
                        events.Add((givenEvent.Event));
                    }
                }
            }
            return events;
        }
    }
}
