using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.Api.Models.Api.Busy
{
    public class BusyTimeResponse
    {
        public DateTime ExpiryDate;
        public List<BusyTime> BusyTimeEntries;
        public class BusyTime
        {
            public string Id;
            public DateTime Start;
            public DateTime? End;
            public short PercentageOccupied;
            public string Comment;
        }
    }
}