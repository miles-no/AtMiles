using System;

namespace Contact.Domain
{
    public interface IRepository<T> where T : AggregateRoot, new()
    {
        void Save(T aggregate, int expectedVersion);
        T GetById(string id);
        T GetById(string id, bool keepHistory);
    }
}
