namespace no.miles.at.Backend.Infrastructure
{
    public interface IHandlePosition
    {
        EventStoreGlobalPosition GetLatestSavedPosition();
        void SaveLatestPosition(EventStoreGlobalPosition position);
    }
}
