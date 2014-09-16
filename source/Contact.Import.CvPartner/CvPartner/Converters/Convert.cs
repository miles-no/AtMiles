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

        public Convert(string company, Person createdBy)
        {
            this.company = company;
            this.createdBy = createdBy;
        }

        public CvPartnerImportData ToImportFromCvPartner(Cv cv, Employee employee, Picture employeePhoto)
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

            DateTime? bornDate = null;
            if (cv.BornYear != null && cv.BornMonth != null && cv.BornDay != null)
            {
                bornDate = new DateTime(cv.BornYear.Value, cv.BornMonth.Value, cv.BornDay.Value);
            }

            var technologies = ConvertCvTechnologies(cv.Technologies);
            var keyQualifications = ConvertCvKeyCompetence(cv.KeyQualifications);
            var res = new CvPartnerImportData(
                firstName: givenName,
                middleName: middleName,
                lastName: familyName,
                dateOfBirth: bornDate,
                email: employee.Email,
                phone: cv.Phone,
                title: cv.Title,
                officeName: employee.OfficeName,
                updatedAt: cv.UpdatedAt,
                keyQualifications: keyQualifications,
                technologies: technologies,
                photo: employeePhoto
            );
            return res;
        }

        private static CvPartnerKeyQualification[] ConvertCvKeyCompetence(IEnumerable<KeyQualification> keyQualifications)
        {
            var convertedQualifications = new List<CvPartnerKeyQualification>();
            if (keyQualifications == null) return convertedQualifications.ToArray();

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

            var res = new AddEmployee(company,
                id,
                new Login(Constants.GoogleIdProvider, employee.Email, null),
                givenName,
                middleName,
                familyName,
                dateOfBirth: bornDate,
                jobTitle: cv.Title,
                officeName: employee.OfficeName,
                phoneNumber: cv.Phone,
                email: employee.Email,
                homeAddress: null,
                photo: employeePhoto,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: new Guid().ToString(), 
                basedOnVersion: Constants.IgnoreVersion);

            return res;
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
    }

}
