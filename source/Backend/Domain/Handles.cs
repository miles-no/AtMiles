using System.Threading.Tasks;

namespace no.miles.at.Backend.Domain
{
    public interface Handles<T> where T : Message
    {
        Task Handle(T message);
    }
}
