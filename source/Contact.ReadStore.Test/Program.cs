using System;
using System.Collections.Generic;
using Contact.Backend.MockStore;

namespace Contact.ReadStore.Test
{
    class Program
    {
        private static void Main(string[] args)
        {
         //   new FillReadStore().Fill();
        }

        //TODO
        public class EventTrackModel
        {
            public string Id { get; set; }
            public string EventType { get; set; }
            public long? LastVersion { get; set; }
        }


        
        //public static void Search(SearchTest search, string searchString)
        //{
        //    Print("\n\nSearching for \"" + searchString + "\"");
        //    var res = search.Search(searchString);
        //    Print(res);

        //}
        //public static void Print(string msg)
        //{
        //    Console.WriteLine(msg);
        //}

        //public static void Print(List<SearchTest.PersonSearchModel> materials)
        //{
        //    Print("Results:\n");
        //    foreach (var searchMaterial in materials)
        //    {
        //     //   Print(searchMaterial.Name + " " + searchMaterial.Name + " (" + string.Join(",",searchMaterial.Tags) + ")");
        //    }
        //}
    }
}
