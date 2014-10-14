using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace no.miles.at.Backend.Domain.Test
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

                var aggregate = new T();

                aggregate.LoadsFromHistory(GetEventsById(id), keepHistory);

                if (aggregate.Id != id)
                {
                    return null;
                }
                return aggregate;
            });
        }

        private Event[] GetEventsById(string id)
        {
            var events = new List<Event>();
            if (GivenEvents != null)
            {
                events.AddRange(from givenEvent in GivenEvents where givenEvent.StreamId == id select (givenEvent.Event));
            }
            return events.ToArray();
        }
    }
}
