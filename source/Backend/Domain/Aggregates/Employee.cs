using System;
using System.Collections.Generic;
using System.Linq;
using Import.Auth0.Model;
using no.miles.at.Backend.Domain.Annotations;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Events.Import;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;

namespace no.miles.at.Backend.Domain.Aggregates
{
    public class Employee : AggregateRoot
    {
        private string _firstName;
        private string _middleName;
        private string _lastName;
        private Login _loginId;
        private DateTime _lastImportUpdateFromCvPartnerAt = DateTime.MinValue;
        private readonly List<BusyTimeEntry> _busyTimeEntries;
        private byte[] _importedPictureMd5Hash;
        private string _lastAuth0Etag = string.Empty;

        public string Name
        {
            get { return NameService.GetName(_firstName, _middleName, _lastName); }
        }

        public Login LoginId { get { return _loginId; } }

        public Employee()
        {
            _busyTimeEntries = new List<BusyTimeEntry>();
        }

        public void CreateNew(string companyId, string companyName, string globalId, Login loginId, string firstName, string middleName, string lastName, Person createdBy, string correlationId)
        {
            var ev = new EmployeeCreated(
                companyId: companyId,
                companyName: companyName,
                employeeId: globalId,
                loginId: loginId,
                firstName: firstName,
                middleName: middleName,
                lastName: lastName,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void Terminate(string companyId, string companyName, Person createdBy, string correlationId)
        {
            var ev = new EmployeeTerminated(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: NameService.GetName(_firstName, _middleName, _lastName),
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void ImportData(string companyId, string companyName, CvPartnerImportData import, Person createdBy, string correlationId)
        {
            if (IsImportDataNew(import))
            {
                var ev = new ImportedFromCvPartner(
                    companyId: companyId,
                    companyName: companyName,
                    employeeId: Id,
                    firstName: import.FirstName,
                    middleName: import.MiddleName,
                    lastName: import.LastName,
                    dateOfBirth: import.DateOfBirth,
                    email: import.Email,
                    phone: import.Phone,
                    title: import.Title,
                    officeName: import.OfficeName,
                    updatedAt: import.UpdatedAt,
                    keyQualifications: import.KeyQualifications,
                    technologies: import.Technologies,
                    photo: import.Photo,
                    created: DateTime.UtcNow,
                    createdBy: createdBy,
                    correlationId: correlationId);
                ApplyChange(ev);
            }
        }
        
        public void EnrichData(EnrichFromAuth0 message, Auth0User user, Company company, Person createdBy, string correlationId)
        {
            if (_lastAuth0Etag != user.Etag)
            {
                var ev = new EnrichedFromAuth0(
                    companyId: company.Id,
                    employeeId: Id,
                    firstName: user.GivenName,
                    lastName: user.FamilyName,
                    etag: user.Etag,
                    email: user.PrimaryEmail,
                    phone: (user.Phones != null && user.Phones.Any()) ? user.Phones.First().Value : null,
                    photo: user.Picture,
                    created: DateTime.UtcNow,
                    createdBy: createdBy,
                    correlationId: correlationId);

                ApplyChange(ev);
            }
        }

        private bool IsImportDataNew(CvPartnerImportData import)
        {
            if (ShouldImportBecauseOfDate(import.UpdatedAt)) return true;
            if (ShouldImportBecauseOfPhoto(import.Photo)) return true;
            return false;
        }

        private bool ShouldImportBecauseOfPhoto(Picture photo)
        {
            if (photo == null && _importedPictureMd5Hash != null) return true;

            if (photo != null)
            {
                if (photo.Md5Hash != null && _importedPictureMd5Hash == null) return true;
                if (photo.Md5Hash == null && _importedPictureMd5Hash != null) return true;
                if (photo.Md5Hash != null && _importedPictureMd5Hash != null)
                {
                    return !photo.Md5Hash.SequenceEqual(_importedPictureMd5Hash);
                }
            }
            return false;
        }

        private bool ShouldImportBecauseOfDate(DateTime importDate)
        {
            return importDate > _lastImportUpdateFromCvPartnerAt;
        }

        public void ConfirmBusyTimeEntries(string companyId, string companyName, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != Id) throw new NoAccessException("Can only confirm busy-time entries on self");

            var ev = new BusyTimeConfirmed(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void AddBusyTime(string companyId, string companyName, DateTime start, DateTime? end, short percentageOccpied, string comment, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != Id) throw new NoAccessException("Can only add busy-time to self");

            if (end.HasValue)
            {
                if (end.Value <= start) throw new ValueException("Start date must be before end date.");
            }

            CheckIfMoreThan100PercentageInAnyPeriode();

            var busyTimeId = IdService.CreateNewId();
            var ev = new BusyTimeAdded(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                busyTimeId: busyTimeId,
                start: start,
                end: end,
                percentageOccpied: percentageOccpied,
                comment: comment,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);

            ApplyChange(ev);
        }

        public void UpdateBusyTimeEnd(string companyId, string companyName, string busyTimeId, DateTime start, DateTime? end, short percentageOccpied, string comment, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != Id) throw new NoAccessException("Can only update busy-time to self");
            
            BusyTimeEntry busyTime = _busyTimeEntries.FirstOrDefault(b => b.Id == busyTimeId);
            if (busyTime == null) throw new UnknownItemException("Unknown ID for Busy time entry");

            if (end.HasValue)
            {
                if (end.Value <= start) throw new ValueException("Start date must be before end date.");
            }

            CheckIfMoreThan100PercentageInAnyPeriode();

            var ev = new BusyTimeUpdated(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                busyTimeId: busyTime.Id,
                start: start,
                end: end,
                percentageOccpied: percentageOccpied,
                comment: comment,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void RemoveBusyTime(string companyId, string companyName, string busyTimeId, Person createdBy, string correlationId)
        {
            if (createdBy.Identifier != Id) throw new NoAccessException("Can only add busy-time to self");

            BusyTimeEntry busyTime = _busyTimeEntries.FirstOrDefault(b => b.Id == busyTimeId);

            if (busyTime == null) throw new UnknownItemException("Unknown ID for Busy time entry");

            var ev = new BusyTimeRemoved(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                busyTimeId: busyTime.Id,
                start: busyTime.Start,
                end: busyTime.End,
                percentageOccpied: busyTime.PercentageOccpied,
                comment: busyTime.Comment,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void SetPrivateAddress(string companyId, string companyName, Address privateAddress, Person createdBy, string correlationId)
        {
            var ev = new PrivateAddressSet(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                privateAddress: privateAddress,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        public void SetDateOfBirth(string companyId, string companyName, DateTime dateOfBirth, Person createdBy, string correlationId)
        {
            var ev = new DateOfBirthSet(
                companyId: companyId,
                companyName: companyName,
                employeeId: Id,
                employeeName: Name,
                dateOfBirth: dateOfBirth,
                created: DateTime.UtcNow,
                createdBy: createdBy,
                correlationId: correlationId);
            ApplyChange(ev);
        }

        private void CheckIfMoreThan100PercentageInAnyPeriode()
        {
            //TODO: Check if more than 100% in any given periode
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EmployeeCreated ev)
        {
            Id = ev.EmployeeId;
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
            _lastImportUpdateFromCvPartnerAt = ev.UpdatedAt;
            if (ev.Photo != null)
            {
                _importedPictureMd5Hash = ev.Photo.Md5Hash;
            }
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(EnrichedFromAuth0 ev)
        {
            _lastAuth0Etag = ev.Etag;
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

        [UsedImplicitly] //To keep resharper happy
        private void Apply(BusyTimeUpdated ev)
        {
            var oldBusyTime = _busyTimeEntries.First(bt => bt.Id == ev.BusyTimeId);
            _busyTimeEntries.RemoveAll(b => b.Id == ev.BusyTimeId);
            var newBusyTime = new BusyTimeEntry(oldBusyTime.Id, ev.Start, ev.End,
                ev.PercentageOccpied, ev.Comment);
            _busyTimeEntries.Add(newBusyTime);
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(DateOfBirthSet ev)
        {
            //Empty for now
        }

        [UsedImplicitly] //To keep resharper happy
        private void Apply(PrivateAddressSet ev)
        {
            //Empty for now
        }
       
    }

    
}
