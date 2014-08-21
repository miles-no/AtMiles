using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using Contact.Domain;
using Contact.Domain.Commands;
using Contact.Domain.Events.Import;
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

        public ImportFromCvPartner ToImportFromCvPartner(string id, Cv cv, Employee employee, Picture employeePhoto, Person createdBy, string correlationId)
        {
            string givenName = string.Empty, middleName = string.Empty;

            var names = cv.Name.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToList();

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

            //TODO: Check for null
            var bornDate = new DateTime(cv.BornYear.Value, cv.BornMonth.Value, cv.BornDay.Value);

            var technologies = ConvertCvTechnologies(cv.Technologies);
            var keyQualifications = ConvertCvKeyCompetence(cv.KeyQualifications);
            var res = new ImportFromCvPartner(givenName, middleName, familyName, bornDate, employee.Email, cv.Phone,
                cv.Title, DateTime.MinValue, keyQualifications, technologies, employeePhoto, DateTime.UtcNow, createdBy, correlationId);

            return res;
        }

        private static CvPartnerKeyQualification[] ConvertCvKeyCompetence(IEnumerable<KeyQualification> keyQualifications)
        {
            var convertedQualifications = new List<CvPartnerKeyQualification>();
            if (keyQualifications != null)
            {
                foreach (var keyQualification in keyQualifications)
                {
                    var internationalDescription = keyQualification.IntLongDescription;
                    var localDescription = keyQualification.LocalLongDescription;

                    var convertedKeyPoints = new List<CvPartnerKeyPoint>();
                    if (keyQualification.KeyPoints != null)
                    {
                        foreach (var keyPoint in keyQualification.KeyPoints)
                        {
                            var kpIntName = keyPoint.IntName;
                            var kpLocalName = keyPoint.LocalName;
                            var kpIntDescription = keyPoint.IntDescription;
                            var kpLocalDescription = keyPoint.LocalDescription;
                            var convertedKeyPoint = new CvPartnerKeyPoint(kpIntName, kpLocalName,kpIntDescription, kpLocalDescription);
                            convertedKeyPoints.Add(convertedKeyPoint);
                        }
                    }
                    var convertedQualification = new CvPartnerKeyQualification(internationalDescription,localDescription, convertedKeyPoints.ToArray());
                    convertedQualifications.Add(convertedQualification);
                }
            }
            return convertedQualifications.ToArray();
        }

        private static CvPartnerTechnology[] ConvertCvTechnologies(IEnumerable<Technology> technologies)
        {
            var convertedTechologies = new List<CvPartnerTechnology>();
            if (technologies == null) return convertedTechologies.ToArray();
            
            foreach (var technology in technologies)
            {
                var internationalCategory = technology.IntCategory;
                var localCategory = technology.LocalCategory;

                var skills = new List<CvPartnerTechnologySkill>();
                if (technology.TechnologySkills != null)
                {
                    foreach (var tag in technology.TechnologySkills)
                    {
                        var intSkill = string.Empty;
                        var localSkill = string.Empty;
                        if (tag.Tags != null)
                        {
                            intSkill = tag.Tags.Int;
                            localSkill = tag.Tags.No;
                        }
                        var skill = new CvPartnerTechnologySkill(intSkill, localSkill);
                        skills.Add(skill);
                    }
                }
                var convertedTechnology = new CvPartnerTechnology(internationalCategory, localCategory, skills.ToArray());
                convertedTechologies.Add(convertedTechnology);
            }
            return convertedTechologies.ToArray();
        }

        public AddEmployee ToAddEmployee(string id, Cv cv, Employee employee, Picture employeePhoto)
        {
            string givenName = string.Empty, middleName = string.Empty;

            var names = cv.Name.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();

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

            CompetenceTag[] competence = ConvertCvToCompetences(cv.Technologies);
            var res = new AddEmployee(company,
                employee.OfficeName,
                id, new Login(Constants.GoogleIdProvider, employee.Email, null), givenName, middleName, familyName,
                bornDate,
                cv.Title, cv.Phone, employee.Email, null, employeePhoto, competence, DateTime.UtcNow, createdBy,
                new Guid().ToString(), Constants.IgnoreVersion);

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
