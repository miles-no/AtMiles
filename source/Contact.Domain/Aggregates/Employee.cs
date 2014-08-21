using System;
using Contact.Domain.Annotations;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;

namespace Contact.Domain.Aggregates
{
    public class Employee : AggregateRoot
    {
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private Login _loginId;
        private DateTime _lastImportUpdateAt = DateTime.MinValue;

        public string Name
        {
            get { return NameService.GetName(_firstName, _middleName, _lastName); }
        }

        public Login LoginId { get { return _loginId; } }

        public void CreateNew(string companyId, string companyName, string officeId, string officeName, string globalId, Login loginId, string firstName, string middleName, string lastName, DateTime? dateOfBirth, string jobTitle, string phoneNumber, string email, Address homeAddress, Picture photo, CompetenceTag[] competence, Person createdBy, string correlationId)
        {
            var ev = new EmployeeCreated(companyId, companyName, officeId, officeName, globalId, loginId, firstName, middleName,
                lastName, dateOfBirth, jobTitle, phoneNumber, email, homeAddress, photo, competence, DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        public void Terminate(string companyId, string companyName, string officeId, string officeName, Person createdBy, string correlationId)
        {
            var ev = new EmployeeTerminated(companyId, companyName, officeId, officeName, _id,
                NameService.GetName(_firstName, _middleName, _lastName),DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        public void ImportData(CvPartnerImportData import, Person createdBy, string correlationId)
        {
            if (import.UpdatedAt > _lastImportUpdateAt)
            {
                var ev = new ImportedFromCvPartner(import.FirstName, import.MiddleName, import.LastName,
                    import.DateOfBirth, import.Email, import.Phone, import.Title, import.UpdatedAt,
                    import.KeyQualifications, import.Technologies, import.Photo, DateTime.UtcNow, createdBy,
                    correlationId);
            }
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeCreated ev)
        {
            _id = ev.GlobalId;
            _firstName = ev.FirstName;
            _middleName = ev.MiddleName;
            _lastName = ev.LastName;
            _loginId = ev.LoginId;
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeTerminated ev)
        {
            //Empty for now. Might be used for soft delete later.
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(ImportedFromCvPartner ev)
        {
            _lastImportUpdateAt = ev.UpdatedAt;
            //Empty for now. Might be used for soft delete later.
        }
    }
}
