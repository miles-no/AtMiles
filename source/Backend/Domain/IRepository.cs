using System.Threading.Tasks;

namespace Contact.Domain
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        Task SaveAsync(T aggregate, int expectedVersion);
        Task<T> GetByIdAsync(string id);
        Task<T> GetByIdAsync(string id, bool keepHistory);
    }
}
