using System.Threading.Tasks;

namespace no.miles.at.Backend.Domain
{
// ReSharper disable once InconsistentNaming
    public interface Handles<in T> where T : Message
    {
        Task Handle(T message);
    }
}
