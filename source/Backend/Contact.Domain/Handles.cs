using System.Threading.Tasks;

namespace Contact.Domain
{
    public interface Handles<T> where T : Message
    {
        Task Handle(T message);
    }
}
