using System;
using System.Collections.Generic;
using Contact.Backend.MockStore;

namespace Contact.ReadStore.Test
{
    class Program
    {
        private static void Main(string[] args)
        {
            var admin = new ReadStoreAdmin();
            admin.PrepareHandlers();
            admin.StartListening();
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
