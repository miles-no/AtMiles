using System;
using Contact.Domain.ValueTypes;

namespace Contact.Domain
{
    public interface Message
    {
        DateTime Created { get; }
        Person CreatedBy { get; }
        String CorrelationId { get; }
    }
}
