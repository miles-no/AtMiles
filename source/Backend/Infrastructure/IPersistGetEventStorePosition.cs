using EventStore.ClientAPI;

namespace no.miles.at.Backend.Infrastructure
{
    public interface IPersistGetEventStorePosition
    {
        Position? GetLastProcessedPosition();
        void PersistLastPositionProcessed(Position? position);
    }
}
