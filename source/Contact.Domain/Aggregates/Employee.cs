namespace Contact.Domain.Aggregates
{
    public class Employee : AggregateRoot
    {
        public string Name { get; private set; }
    }
}
