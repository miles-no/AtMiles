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
    public class ImportDataFromCvPartnerNewUserAndNewOfficeTest : EventSpecification<ImportDataFromCvPartner>
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

        private DateTime _timestamp11 = DateTime.MinValue;

        private DateTime _timestamp21 = DateTime.MinValue;
        private DateTime _timestamp22 = DateTime.MinValue;

        public string EmployeeId = string.Empty;

        private readonly CvPartnerImportData _importData = new CvPartnerImportData(
            firstName: "Ole",
            middleName: string.Empty,
            lastName: "Olsen",
            dateOfBirth: new DateTime(1990, 1, 1),
            email: "ole.olsen@miles.no",
            phone: "123456789",
            title: "Senior Consultant",
            officeName: "Bergen",
            updatedAt: new DateTime(2014, 6, 6),
            keyQualifications: new[] { new CvPartnerKeyQualification("Tester", "Tester", null) },
            technologies: null,
            photo: null
        );

        [Test]
        public async void import_data_from_CvPartner_new_user_and_new_office()
        {
            await Setup();
        }

        public override IEnumerable<Event> Produced()
        {

            var events1 = _fakeCompanyRepository.GetThenEvents();
            if (events1.Count == 1)
            {
                _timestamp11 = events1[0].Created;
            }

            var events2 = _fakeEmployeeRepository.GetThenEvents();
            if (events2.Count == 2)
            {
                _timestamp21 = events2[0].Created;
                _timestamp22 = events2[1].Created;

                EmployeeId = ((EmployeeCreated)events2[0]).EmployeeId;
            }

            events1.AddRange(events2);
            return events1;
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
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), _correlationId)),
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

        public override ImportDataFromCvPartner When()
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

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new EmployeeAdded(CompanyId, CompanyName,EmployeeId,NameService.GetName(_importData.FirstName, _importData.MiddleName, _importData.LastName),new Login(Constants.GoogleIdProvider, _importData.Email),_timestamp11, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new EmployeeCreated(CompanyId, CompanyName, EmployeeId, new Login(Constants.GoogleIdProvider,_importData.Email), _importData.FirstName, _importData.MiddleName, _importData.LastName, _timestamp21, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId),
                    new ImportedFromCvPartner(CompanyId, CompanyName, EmployeeId, _importData.FirstName, _importData.MiddleName, _importData.LastName, _importData.DateOfBirth ,_importData.Email, _importData.Phone, _importData.Title, _importData.OfficeName, _importData.UpdatedAt, _importData.KeyQualifications, _importData.Technologies, _importData.Photo, _timestamp22, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)
                };
            return events;
        }
    }
}
