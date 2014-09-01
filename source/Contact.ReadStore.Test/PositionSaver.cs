using Raven.Client;

namespace Contact.ReadStore
{
    public class PositionSaver
    {
        private const string PositionId = "global/POSITION";
        private readonly IDocumentStore _documentStore;

        public PositionSaver(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public EventStoreGlobalPosition GetLatestSavedPosition()
        {
            using (var session = _documentStore.OpenSession())
            {
                var position = session.Load<EventStoreGlobalPosition>(PositionId);
                return position;
            }
        }

        public void SaveLatestPosition(EventStoreGlobalPosition position)
        {
            position.Id = PositionId;
            using (var session = _documentStore.OpenSession())
            {
                session.Store(position);
                session.SaveChanges();
            }
        }
    }
}
