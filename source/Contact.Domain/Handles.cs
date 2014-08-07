namespace Contact.Domain
{
    public interface Handles<T> where T : Message
    {
        void Handle(T message);
    }
}
