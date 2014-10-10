using System;
using System.Collections.Generic;

namespace Contact.Backend.Models.Api.Busy
{
    public class BusyTimeResponse
    {
        public DateTime ExpiryDate { get; set; }
        public List<BusyTime> BusyTimeEntries { get; set; }
        public class BusyTime
        {
            public string Id { get; set; }
            public DateTime Start { get; set; }
            public DateTime? End { get; set; }
            public short PercentageOccupied { get; set; }
            public string Comment { get; set; }
        }
    }
}