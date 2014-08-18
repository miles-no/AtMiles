using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Contact.Domain;
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
            if (cv.Image != null && cv.Image.Url != null)
            {
                byte[] picture = null;
                string extension = null;

                try
                {
                    picture = new WebClient().DownloadData(cv.Image.Url);
                    var urlWithoutQueryParameters = cv.Image.Url.Substring(0, cv.Image.Url.IndexOf("?"));
                    extension = urlWithoutQueryParameters.Substring(cv.Image.Url.LastIndexOf("."))
                        .Replace(".", string.Empty);

                    log("Found image of " + "." + extension + " format");
                }
                catch (Exception ex)
                {
                    log("Error downloading image:\n\n " + ex);

                }
                if (picture != null)
                {
                    employeePhoto = new Picture(employee.Name,extension,picture);
                }
            }

            //TODO: Include competence-tags

            CompetenceTag[] competence = ConvertCvToCompetences(cv.Technologies);
            var res = new AddEmployee(company,
                employee.OfficeName,
                id, new Login(Constants.GoogleIdProvider, employee.Email, null), givenName, middleName, familyName,
                bornDate,
                cv.Title, cv.Telefon, employee.Email, null, employeePhoto, competence, DateTime.UtcNow, createdBy,
                new Guid().ToString(), Domain.Constants.IgnoreVersion);

            return res;
        }

        private static CompetenceTag[] ConvertCvToCompetences(IEnumerable<Technology> technologies)
        {
            var competences = new List<CompetenceTag>();
            foreach (var technology in technologies)
            {
                int intTagsCount = technology.IntTags != null ? technology.IntTags.Length : 0;
                int localTagsCount = technology.LocalTags != null ? technology.LocalTags.Length : 0;

                int max = Math.Max(intTagsCount, localTagsCount);

                for (int i = 0; i < max; i++)
                {
                    string intTag = GetTagFromArray(i, technology.IntTags);
                    string localTag = GetTagFromArray(i, technology.LocalTags);
                    var tag = new CompetenceTag(technology.LocalCategory, technology.IntCategory, localTag, intTag);
                    competences.Add(tag);
                }
            }

            return competences.ToArray();
        }

        private static string GetTagFromArray(int position, string[] tags)
        {
            if (tags == null) return String.Empty;
            if (position < 0 || position >= tags.Length - 1) return string.Empty;
            return tags[position];
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
