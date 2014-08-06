using System;

namespace Contact.Domain
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(T aggregate, int expectedVersion);
        T GetById(Guid id);
        T GetById(Guid id, bool keepHistory);
    }
}
