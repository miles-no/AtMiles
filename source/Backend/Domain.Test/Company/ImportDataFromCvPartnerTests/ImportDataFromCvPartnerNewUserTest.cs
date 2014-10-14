using System;
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
    public class ImportDataFromCvPartnerNewUserTest : EventSpecification<ImportDataFromCvPartner>
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

        private DateTime _timestamp1 = DateTime.MinValue;
        private DateTime _timestamp2 = DateTime.MinValue;

        private string _employeeId = string.Empty;

        private readonly CvPartnerImportData _importData = new CvPartnerImportData(
            firstName: "Ole",
            middleName: string.Empty,
            lastName: "Olsen",
            dateOfBirth: new DateTime(1990, 1, 1),
            email: "ole.olsen@miles.no",
            phone: "123456789",
            title: "Senior Consultant",
            officeName: "Stavanger",
            updatedAt: new DateTime(2014, 6, 6),
            keyQualifications: new[] { new CvPartnerKeyQualification("Tester", "Tester", null) },
            technologies: null,
            photo: null
        );

        [Test]
        public async void import_data_from_CvPartner_new_user()
        {
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            var events = _fakeEmployeeRepository.GetThenEvents();
            if (events.Count == 2)
            {
                _timestamp1 = events[0].Created;
                _timestamp2 = events[1].Created;

                _employeeId = ((EmployeeCreated) events[0]).EmployeeId;
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
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName,   AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId)),
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),"INIT")),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId, NameService.GetName(AdminFirstName, AdminLastName), DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId),_correlationId)),
                };
            return events;
        }

        protected override ImportDataFromCvPartner When()
        {
            return new ImportDataFromCvPartner(CompanyId, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<ImportDataFromCvPartner> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeGlobalRepository = new FakeRepository<Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>(new[] { _importData }));
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter);
        }

        protected override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new EmployeeCreated(CompanyId, CompanyName, _employeeId, new Login(Constants.GoogleIdProvider,_importData.Email), _importData.FirstName, _importData.MiddleName, _importData.LastName, _timestamp1, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new ImportedFromCvPartner(CompanyId, CompanyName, _employeeId, _importData.FirstName, _importData.MiddleName, _importData.LastName, _importData.DateOfBirth ,_importData.Email, _importData.Phone, _importData.Title, _importData.OfficeName, _importData.UpdatedAt, _importData.KeyQualifications, _importData.Technologies, _importData.Photo, _timestamp2, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
