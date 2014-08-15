using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Contact.Backend.MockStore;

namespace Contact.ReadStore.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new SearchTest();
            //test.Setup();
            //Search(test,"Jack");
            //Search(test, "Banana");
            //Search(test, "Dessert Jack");

            var fetcher = new EventFetcher();
            fetcher.Subscribe(async (e) =>
            {
                lock("updateInSeqence")
                {
                    Console.WriteLine(e.Event.EventType);
                    var json = Encoding.UTF8.GetString(e.Event.Data);
                }

            }, new[] { "EmployeeAdded" });

            Console.ReadLine();
        }


        public class PersonSearchModel
        {
            public string CompanyId { get; set; }
            public string OfficeId { get; set; }
            public string GlobalId { get; set; }
            public string Name { get; set; }

            public readonly string FirstName;
            public readonly string MiddleName;
            public readonly string LastName;
            public readonly DateTime DateOfBirth;
            public readonly string JobTitle;
            public readonly string PhoneNumber;
            public readonly string Email;
            public readonly string Thumb;
        }

        public class EmployeeAddedModel
        {
            public string CompanyId { get; set; }
            public string OfficeId { get; set; }
            public string GlobalId { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string JobTitle { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public Picture Photo { get; set; }
        }

        public class Picture
        {
            public string Title { get; set; }
            public string Extension { get; set; }
            public byte[] Content { get; set; }
        }

        public static void Search(SearchTest search, string searchString)
        {
            Print("\n\nSearching for \"" + searchString + "\"");
            var res = search.Search(searchString);
            Print(res);

        }
        public static void Print(string msg)
        {
            Console.WriteLine(msg);
        }

        public static void Print(List<SearchTest.SearchMaterial> materials)
        {
            Print("Results:\n");
            foreach (var searchMaterial in materials)
            {
                Print(searchMaterial.FirstName + " " + searchMaterial.LastName + " (" + string.Join(",",searchMaterial.Tags) + ")");
            }
        }
    }
}
