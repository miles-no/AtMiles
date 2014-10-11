using EventStore.ClientAPI;

namespace Contact.Infrastructure
{
    public interface IPersistGetEventStorePosition
    {
        Position? GetLastProcessedPosition();
        void PersistLastPositionProcessed(Position? position);
    }
}
