﻿using System;
using System.Collections.Generic;
using Contact.Domain.Aggregates;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.ImportDataFromCvPartnerTests
{
    [TestFixture]
    public class ImportDataFromCvPartnerExistingUserWithUpdatesTest : EventSpecification<ImportDataFromCvPartner>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeRepository<Aggregates.Global> _fakeGlobalRepository;
        private FakeCvPartnerImporter _fakeImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";

        public readonly string EmployeeId = "emp1";

        private readonly CvPartnerImportData _importData1 = new CvPartnerImportData
        {
            FirstName = "Ole",
            MiddleName = string.Empty,
            LastName = "Olsen",
            DateOfBirth = new DateTime(1990,1,1),
            Email = "ole.olsen@miles.no",
            Phone = "123456789",
            Title = "Senior Promblemmaker",
            UpdatedAt = new DateTime(2014,5,5),
            KeyQualifications = null,
            OfficeName = "Stavanger",
            Technologies = null
        };

        private readonly CvPartnerImportData _importData2 = new CvPartnerImportData
        {
            FirstName = "Ole",
            MiddleName = string.Empty,
            LastName = "Olsen",
            DateOfBirth = new DateTime(1990, 1, 1),
            Email = "ole.olsen@miles.no",
            Phone = "123456789",
            Title = "Senior Consultant",
            UpdatedAt = new DateTime(2014, 6, 6),
            KeyQualifications = new[]{new CvPartnerKeyQualification("Tester", "Tester", null)},
            OfficeName = "Stavanger",
            Technologies = null
        };

        private DateTime _timestamp = DateTime.MinValue;

        [Test]
        public void import_data_from_CvPartner_existing_user_with_updates()
        {
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events = _fakeEmployeeRepository.GetThenEvents();
            if (events.Count == 1)
            {
                _timestamp = events[0].Created;
            }
            return events;
        }

        public override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var g = GivenGlobal();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            all.AddRange(g);
            return all;
        }

        private IEnumerable<FakeStreamEvent> GivenGlobal()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Global.GlobalId, new Events.Global.CompanyCreated(CompanyId, CompanyName,new DateTime(2014,1,1),new Person(Constants.SystemUserId, Constants.SystemUserId), "SYSTEM INIT" ))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, null, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId)),
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId, new Login(Constants.GoogleIdProvider,_importData1.Email, string.Empty), _importData1.FirstName, _importData1.MiddleName, _importData1.LastName, _importData1.DateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    new FakeStreamEvent(EmployeeId, new Events.Import.ImportedFromCvPartner(EmployeeId, _importData1.FirstName, _importData1.MiddleName, _importData1.LastName, _importData1.DateOfBirth,_importData1.Email, _importData1.Phone, _importData1.Title, _importData1.UpdatedAt, _importData1.KeyQualifications, _importData1.Technologies, _importData1.Photo,DateTime.UtcNow, new Person(AdminId, Domain.Services.NameService.GetName(AdminFirstName, AdminLastName)), "IMPORT1"))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),"INIT")),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName, AdminLastName), DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),_correlationId)),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId, NameService.GetName(_importData1.FirstName, _importData1.MiddleName, _importData1.LastName), new Login(Constants.GoogleIdProvider, _importData1.Email, string.Empty),DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    
                };
            return events;
        }

        public override ImportDataFromCvPartner When()
        {
            return new ImportDataFromCvPartner(CompanyId, DateTime.UtcNow, new Person(AdminId, Services.NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<ImportDataFromCvPartner> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Aggregates.Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>(new[]{_importData2} ));
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new Events.Import.ImportedFromCvPartner(EmployeeId, _importData2.FirstName, _importData2.MiddleName, _importData2.LastName, _importData2.DateOfBirth ,_importData2.Email, _importData2.Phone, _importData2.Title, _importData2.UpdatedAt, _importData2.KeyQualifications, _importData2.Technologies, _importData2.Photo, _timestamp, new Person(AdminId, Domain.Services.NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
