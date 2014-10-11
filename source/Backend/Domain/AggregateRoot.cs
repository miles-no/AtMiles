using System.Collections.Generic;

namespace no.miles.at.Backend.Domain
{
    public abstract class AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();
        private readonly List<Event> _history = new List<Event>();

        protected string _id;

        public string Id
        {
            get { return _id; }
        }
        public int Version { get; internal set; }
        public Event[] HistoryEvents { get { return _history.ToArray(); } }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history, bool keepHistory = false)
        {
            foreach (var e in history)
            {
                ApplyChange(e, false);
            }
            if (keepHistory)
            {
                _history.AddRange(history);
            }
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew) _changes.Add(@event);
        }
    }
}
