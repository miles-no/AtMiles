using System;
using System.Collections.Generic;

namespace no.miles.at.Backend.ReadStore.BusyTimeStore
{
    public class BusyTimeModel
    {
        public string Id { get; set; }
        public DateTime ExpiryDate { get; set; }
        private List<BusyTime> _busyTimeEntries = new List<BusyTime>();

        public List<BusyTime> BusyTimeEntries
        {
            get { return _busyTimeEntries; }
            set { _busyTimeEntries = value; }
        }

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
