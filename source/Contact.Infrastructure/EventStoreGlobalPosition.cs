namespace Contact.Infrastructure
{
    public class EventStoreGlobalPosition
    {
        public long PreparePosition { get; set; }
        public long CommitPosition { get; set; }
    }
}
