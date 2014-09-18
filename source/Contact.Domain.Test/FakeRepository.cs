using System.Collections.Generic;
using System.Threading.Tasks;

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


        public async Task SaveAsync(T aggregate, int expectedVersion)
        {
            await Task.Run(() =>
            {
                var eventsToSave = aggregate.GetUncommittedChanges();
                ThenEvents.AddRange(eventsToSave);
                aggregate.MarkChangesAsCommitted();
            });
        }

        public async Task<T> GetByIdAsync(string id)
        {
            return await GetByIdAsync(id, false);
        }

        public async Task<T> GetByIdAsync(string id, bool keepHistory)
        {
            return await Task.Run(() =>
            {
                if (id == string.Empty)
                {
                    return null;
                }

                T aggregate = new T();

                aggregate.LoadsFromHistory(GetEventsById(id), keepHistory);

                if (aggregate.Id != id)
                {
                    return null;
                }
                return aggregate;
            });
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
