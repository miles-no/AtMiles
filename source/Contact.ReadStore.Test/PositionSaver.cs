using Contact.Infrastructure;
using Raven.Client;

namespace Contact.ReadStore
{
    public class PositionSaver : IHandlePosition
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
                var position = session.Load<EventStoreGlobalPositionUsedInRavenDb>(PositionId);
                if (position == null) return null;
                return new EventStoreGlobalPosition
                {
                    PreparePosition = position.PreparePosition,
                    CommitPosition = position.CommitPosition
                };
            }
        }

        public void SaveLatestPosition(EventStoreGlobalPosition position)
        {
            var toSave = new EventStoreGlobalPositionUsedInRavenDb
            {
                Id = PositionId,
                PreparePosition = position.PreparePosition,
                CommitPosition = position.CommitPosition
            };
            using (var session = _documentStore.OpenSession())
            {
                session.Store(toSave);
                session.SaveChanges();
            }
        }
    }
}
