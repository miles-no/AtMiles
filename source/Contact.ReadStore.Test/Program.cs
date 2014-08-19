using System;
using System.Collections.Generic;
using Contact.Backend.MockStore;

namespace Contact.ReadStore.Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            new FillReadStore().FillAndPrepare();
        }

        //TODO
        public class EventTrackModel
        {
            public string Id { get; set; }
            public string EventType { get; set; }
            public long? LastVersion { get; set; }
        }


       
    }
}
