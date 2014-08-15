using System;
using System.Linq;
using System.Net;
using Contact.Domain.Commands;
using Contact.Domain.ValueTypes;
using Contact.Import.CvPartner.CvPartner.Models.Cv;
using Contact.Import.CvPartner.CvPartner.Models.Employee;

namespace Contact.Import.CvPartner.CvPartner.Converters
{
    public class Convert

    {
        private readonly string company;
        private readonly Person createdBy;
        private readonly Action<string> log;

        public Convert(string company, Person createdBy, Action<string> log)
        {
            this.company = company;
            this.createdBy = createdBy;
            this.log = log;
        }


        public AddEmployee ToAddEmployee(string id, Cv cv, Employee employee)
        {
            string givenName = string.Empty, middleName = string.Empty;

            var names = cv.Navn.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();

            string familyName = names.Last();

            names = names.Take(names.Count() - 1).ToList();

            // If you have 3 or more names, lets assume the second-to-last is your middlename. And hope we don't offent to many...
            if (names.Count() > 2)
            {
                middleName = names.Last();
            }
            else
            {
                givenName = string.Join(" ", names);
            }

            var bornDate = new DateTime(cv.BornYear.Value, cv.BornMonth.Value, cv.BornDay.Value);

            Picture employeePhoto = null;
            if (employee.Image != null && employee.Image.Url != null)
            {
                var picture = new WebClient().DownloadData(employee.Image.Url);
                var urlWithoutQueryParameters = employee.Image.Url.Substring(0, employee.Image.Url.IndexOf("?"));
                var extension = urlWithoutQueryParameters.Substring(employee.Image.Url.LastIndexOf(".")).Replace(".",string.Empty);
                
                log("Found image of " + "." + extension + " format");
                employeePhoto = new Picture(employee.Name,extension,picture);
            }
            
            var res = new AddEmployee(company,
                employee.OfficeName,
                id, new Login("Google", employee.Email, null), givenName, middleName, familyName,
                bornDate,
                cv.Title, cv.Telefon, employee.Email, null, employeePhoto, DateTime.UtcNow, createdBy,
                new Guid().ToString(), Domain.Constants.IgnoreVersion);

            return res;
        }

        public byte[] GetPicture(string url)
        {
            var client = new WebClient();
            return client.DownloadData(url);
        }

        public OpenOffice ToOpenOffice(string officeName)
        {
            return new OpenOffice(company, officeName, officeName, null, DateTime.UtcNow, createdBy, Guid.NewGuid().ToString(), Domain.Constants.IgnoreVersion);
        }
    }

}
