﻿using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.Aggregates;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Events.Import;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Company.ImportDataFromCvPartnerTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class ImportDataFromCvPartnerExistingUserWithUpdatesTest : EventSpecification<ImportDataFromCvPartner>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();

        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeRepository<Global> _fakeGlobalRepository;
        private FakeCvPartnerImporter _fakeImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";

        private const string EmployeeId = "emp1";

        private readonly CvPartnerImportData _importData1 = new CvPartnerImportData(
            firstName: "Ole",
            middleName: string.Empty,
            lastName: "Olsen",
            dateOfBirth: new DateTime(1990,1,1),
            email: "ole.olsen@miles.no",
            phone: "123456789",
            title: "Senior Promblemmaker",
            officeName: "Stavanger",
            updatedAt: new DateTime(2014,5,5),
            keyQualifications: null,
            technologies: null,
            projects: null,
            photo: null
        );

        private readonly CvPartnerImportData _importData2 = new CvPartnerImportData(
            firstName: "Ole",
            middleName: string.Empty,
            lastName: "Olsen",
            dateOfBirth: new DateTime(1990, 1, 1),
            email: "ole.olsen@miles.no",
            phone: "123456789",
            title: "Senior Consultant",
            officeName: "Stavanger",
            updatedAt: new DateTime(2014, 6, 6),
            keyQualifications: new[]{new CvPartnerKeyQualification("Tester", "Tester", null)},
            technologies: null,
            projects: null,
            photo: null
        );

        private DateTime _timestamp = DateTime.MinValue;

        [Test]
        public async void import_data_from_CvPartner_existing_user_with_updates()
        {
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            var events = _fakeEmployeeRepository.GetThenEvents();
            if (events.Count == 1)
            {
                _timestamp = events[0].Created;
            }
            return events;
        }

        protected override IEnumerable<FakeStreamEvent> Given()
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
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId)),
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, EmployeeId, new Login(Constants.GoogleIdProvider,_importData1.Email), _importData1.FirstName, _importData1.MiddleName, _importData1.LastName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    new FakeStreamEvent(EmployeeId, new ImportedFromCvPartner(CompanyId, CompanyName, EmployeeId, _importData1.FirstName, _importData1.MiddleName, _importData1.LastName, _importData1.DateOfBirth,_importData1.Email, _importData1.Phone, _importData1.Title, _importData1.OfficeName, _importData1.UpdatedAt, _importData1.KeyQualifications, _importData1.Technologies, _importData1.Projects, _importData1.Photo,DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), "IMPORT1"))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),"INIT")),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName, AdminLastName), DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),_correlationId)),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeId, NameService.GetName(_importData1.FirstName, _importData1.MiddleName, _importData1.LastName), new Login(Constants.GoogleIdProvider, _importData1.Email),DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    
                };
            return events;
        }

        protected override ImportDataFromCvPartner When()
        {
            return new ImportDataFromCvPartner(CompanyId, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, Constants.IgnoreVersion);
        }

        protected override Handles<ImportDataFromCvPartner> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>(new[]{_importData2} ));
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter, new FakeEnrichFromAuth0());
        }

        protected override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new ImportedFromCvPartner(CompanyId, CompanyName, EmployeeId, _importData2.FirstName, _importData2.MiddleName, _importData2.LastName, _importData2.DateOfBirth ,_importData2.Email, _importData2.Phone, _importData2.Title, _importData2.OfficeName, _importData2.UpdatedAt, _importData2.KeyQualifications, _importData2.Technologies, _importData2.Projects, _importData2.Photo, _timestamp, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
