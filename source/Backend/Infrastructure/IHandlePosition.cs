namespace Contact.Infrastructure
{
    public interface IHandlePosition
    {
        EventStoreGlobalPosition GetLatestSavedPosition();
        void SaveLatestPosition(EventStoreGlobalPosition position);
    }
}
