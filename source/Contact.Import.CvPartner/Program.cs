using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Contact.Import.CvPartner
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new WebClient();
            client.Headers[HttpRequestHeader.Authorization] = "Token token=\"removed\"";

            var employees = JsonConvert.DeserializeObject<List<Employee>>(client.DownloadString("https://miles.cvpartner.no/api/v1/users"));

            var distinctOffices = employees.Select(s => s.office_name).Distinct().ToList();

            Console.WriteLine("Nr of employees: " + employees.Count);
            Console.WriteLine("Nr of employees in mumbai: " + employees.First(w=>w.office_name=="Mumbai").name);
            
            foreach (var distinctOffice in distinctOffices)
            {
                Console.WriteLine(distinctOffice);
            }
            var first = employees.First();
            var url = "https://miles.cvpartner.no/api/v1/cvs/" + first.user_id + "/" +
                      first.default_cv_id;
            var cv =
                JsonConvert.DeserializeObject<Cv.Cv>(client.DownloadString(url));


        }
    }

    public class Thumb
    {
        public string url { get; set; }
    }

    public class FitThumb
    {
        public string url { get; set; }
    }

    public class SmallThumb
    {
        public string url { get; set; }
    }

    public class Image
    {
        public string url { get; set; }
        public Thumb thumb { get; set; }
        public FitThumb fit_thumb { get; set; }
        public SmallThumb small_thumb { get; set; }
    }

    public class Employee
    {
        public string user_id { get; set; }
        public string _id { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public string telephone { get; set; }
        public string role { get; set; }
        public string office_id { get; set; }
        public string office_name { get; set; }
        public string country_id { get; set; }
        public string country_code { get; set; }
        public string international_toggle { get; set; }
        public string preferred_download_format { get; set; }
        public List<string> masterdata_languages { get; set; }
        public bool expand_proposals_toggle { get; set; }
        public string default_cv_id { get; set; }
        public string name { get; set; }
        public Image image { get; set; }
    }
}
