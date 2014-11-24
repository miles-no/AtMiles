using System;
using Import.Auth0.Model;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Events.Import
{
    public class ImportedFromCvPartner : Event
    {
        public readonly string CompanyId;
        public readonly string CompanyName;
        public readonly string EmployeeId;
        public readonly string FirstName;
        public readonly string MiddleName;
        public readonly string LastName;
        public readonly DateTime? DateOfBirth;
        public readonly string Email;
        public readonly string Phone;
        public readonly string Title;
        public readonly string OfficeName;
        public readonly DateTime UpdatedAt;
        public readonly CvPartnerKeyQualification[] KeyQualifications;
        public readonly CvPartnerTechnology[] Technologies;
        public readonly CvPartnerProjectInfo[] Projects;
        public readonly Picture Photo;



        public ImportedFromCvPartner(string companyId, string companyName,
            string employeeId, string firstName, string middleName, string lastName, DateTime? dateOfBirth, string email, string phone, string title, string officeName, DateTime updatedAt, CvPartnerKeyQualification[] keyQualifications, CvPartnerTechnology[] technologies, CvPartnerProjectInfo[] projects, Picture photo, DateTime created, Person createdBy, string correlationId) :
            base(created, createdBy, correlationId)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            EmployeeId = employeeId;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Email = email;
            Phone = phone;
            Title = title;
            OfficeName = officeName;
            UpdatedAt = updatedAt;
            KeyQualifications = keyQualifications;
            Technologies = technologies;
            Projects = projects;
            Photo = photo;
        }
    }
}
