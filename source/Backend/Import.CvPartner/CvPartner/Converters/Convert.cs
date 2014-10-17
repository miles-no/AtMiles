using System;
using System.Collections.Generic;
using System.Linq;
using no.miles.at.Backend.Domain.ValueTypes;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Cv;
using no.miles.at.Backend.Import.CvPartner.CvPartner.Models.Employee;

namespace no.miles.at.Backend.Import.CvPartner.CvPartner.Converters
{
    public class Convert

    {
        private readonly string _company;
        private readonly Person _createdBy;

        public Convert(string company, Person createdBy)
        {
            _company = company;
            _createdBy = createdBy;
        }

        public CvPartnerImportData ToImportFromCvPartner(Cv cv, Employee employee, Picture employeePhoto)
        {
            string givenName = string.Empty, middleName = string.Empty, familyName = string.Empty;



            if (!string.IsNullOrEmpty(cv.Name))
            {
                var names = cv.Name.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries).ToList();

                familyName = names.Last();

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
                    convertedKeyPoints.AddRange(
                        from keyPoint in keyQualification.KeyPoints
                        let kpIntName = keyPoint.IntName
                        let kpLocalName = keyPoint.LocalName
                        let kpIntDescription = keyPoint.IntDescription
                        let kpLocalDescription = keyPoint.LocalDescription
                        select new CvPartnerKeyPoint(kpIntName, kpLocalName, kpIntDescription, kpLocalDescription));
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
    }
}
