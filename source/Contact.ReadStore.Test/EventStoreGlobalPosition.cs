namespace Contact.ReadStore
{
    public class EventStoreGlobalPosition
    {
        public string Id { get; set; }
        public long PreparePosition { get; set; }
        public long CommitPosition { get; set; }
    }
}
