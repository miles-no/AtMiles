﻿using System.Runtime.Serialization;

namespace Contact.Domain.Exceptions
{
    public class ValueException : DomainBaseException
    {
        public ValueException(string message) : base(message) { }
        public ValueException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public override string GetExceptionName()
        {
            return "Value exception";
        }
    }
}
