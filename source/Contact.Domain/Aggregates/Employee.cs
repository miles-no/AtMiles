using System;
using System.Collections.Generic;
using System.Linq;
using Contact.Domain.Annotations;
using Contact.Domain.Events.Employee;
using Contact.Domain.Events.Import;
using Contact.Domain.Exceptions;
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
        private List<BusyTimeEntry> _busyTimeEntries;

        public string Name
        {
            get { return NameService.GetName(_firstName, _middleName, _lastName); }
        }

        public Login LoginId { get { return _loginId; } }

        public Employee()
        {
            _busyTimeEntries = new List<BusyTimeEntry>();
        }

        public void CreateNew(string companyId, string companyName, string officeId, string officeName, string globalId, Login loginId, string firstName, string middleName, string lastName, DateTime? dateOfBirth, string jobTitle, string phoneNumber, string email, Address homeAddress, Picture photo, Person createdBy, string correlationId)
        {
            var ev = new EmployeeCreated(companyId, companyName, officeId, officeName, globalId, loginId, firstName, middleName,
                lastName, dateOfBirth, jobTitle, phoneNumber, email, homeAddress, photo, DateTime.UtcNow, createdBy, correlationId);
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
                var ev = new ImportedFromCvPartner(_id, import.FirstName, import.MiddleName, import.LastName,
                    import.DateOfBirth, import.Email, import.Phone, import.Title, import.UpdatedAt,
                    import.KeyQualifications, import.Technologies, import.Photo, DateTime.UtcNow, createdBy,
                    correlationId);
                ApplyChange(ev);
            }
        }

        public void ConfirmBusyTimeEntries(string companyId, string companyName, string officeId, string officeName, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != _id) throw new NoAccessException("Can only confirm busy-time entries on self");

            var ev = new BusyTimeConfirmed(companyId, companyName, officeId, officeName, _id, Name, DateTime.UtcNow, createdBy, correlationId);
            ApplyChange(ev);
        }

        public void AddBusyTime(string companyId, string companyName, string officeId, string officeName, DateTime start, DateTime? end, short percentageOccpied, string comment, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != _id) throw new NoAccessException("Can only add busy-time to self");

            if (end.HasValue)
            {
                if (end.Value <= start) throw new ValueException("Start date must be before end date.");
            }

            if (AnyConflictWithExistingBusyTimeEntries(start, end)) throw new AlreadyExistingItemException("Existing busy time items already defined in this range.");

            var busyTimeId = Services.IdService.CreateNewId();
            var ev = new BusyTimeAdded(companyId, companyName, officeId, officeName, _id, Name, busyTimeId, start, end,
                percentageOccpied, comment, DateTime.UtcNow, createdBy, correlationId);

            ApplyChange(ev);
        }

        public void RemoveBusyTime(string companyId, string companyName, string officeId, string officeName, string busyTimeId, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != _id) throw new NoAccessException("Can only add busy-time to self");

            BusyTimeEntry busyTime = _busyTimeEntries.FirstOrDefault(b => b.Id == busyTimeId);

            if (busyTime == null) throw new UnknownItemException("Unknown ID for Busy time entry");

            var ev = new BusyTimeRemoved(companyId, companyName, officeId, officeName, _id, Name, busyTime.Id,
                busyTime.Start, busyTime.End, busyTime.PercentageOccpied, busyTime.Comment, DateTime.UtcNow, createdBy, correlationId);

            ApplyChange(ev);
        }

        private bool AnyConflictWithExistingBusyTimeEntries(DateTime start, DateTime? end)
        {
            foreach (var busyTimeEntry in _busyTimeEntries)
            {
                DateTime startExisting = busyTimeEntry.Start;
                DateTime endExisting = busyTimeEntry.End.HasValue ? busyTimeEntry.End.Value : DateTime.MaxValue;

                DateTime endConverted = end.HasValue ? end.Value : DateTime.MaxValue;

                if (TimePeriodOverlap(startExisting, endExisting, start, endConverted))
                {
                    return true;
                }
            }
            return false;
        }

        public bool TimePeriodOverlap(DateTime BS, DateTime BE, DateTime TS, DateTime TE)
        {
            return (
                // 1. Case:
                //
                //       TS-------TE
                //    BS------BE 
                //
                // TS is after BS but before BE
                (TS >= BS && TS < BE)
                || // or

                // 2. Case
                //
                //    TS-------TE
                //        BS---------BE
                //
                // TE is before BE but after BS
                (TE <= BE && TE > BS)
                || // or

                // 3. Case
                //
                //  TS----------TE
                //     BS----BE
                //
                // TS is before BS and TE is after BE
                (TS <= BS && TE >= BE)
            );
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
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(BusyTimeAdded ev)
        {
            _busyTimeEntries.Add(new BusyTimeEntry(ev.BusyTimeId,ev.Start,ev.End, ev.PercentageOccpied, ev.Comment));
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(BusyTimeRemoved ev)
        {
            _busyTimeEntries.RemoveAll(b => b.Id == ev.BusyTimeId);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(BusyTimeConfirmed ev)
        {
            //Empty for now
        }
    }
}
