using System;

namespace Contact.Domain.ValueTypes
{
    public class BusyTimeEntry
    {
        public readonly string Id;
        public readonly DateTime Start;
        public readonly DateTime? End;
        public readonly short PercentageOccpied;
        public readonly string Comment;

        public BusyTimeEntry(string id, DateTime start, DateTime? end, short percentageOccpied, string comment)
        {
            Id = id;
            Start = start;
            End = end;
            PercentageOccpied = percentageOccpied;
            Comment = comment;
        }
    }
}
