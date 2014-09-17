using System.Threading.Tasks;

namespace Contact.Domain
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        Task SaveAsync(T aggregate, int expectedVersion);
        T GetById(string id);
        T GetById(string id, bool keepHistory);
    }
}
