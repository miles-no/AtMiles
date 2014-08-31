using System;
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
    public class ImportDataFromCvPartnerExistingUserNoUpdatesTest : EventSpecification<ImportDataFromCvPartner>
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
        public void import_data_from_CvPartner_existing_user_no_updates()
        {
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            yield break;
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
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId, new Login(Constants.GoogleIdProvider,_importData.Email, string.Empty), _importData.FirstName, _importData.MiddleName, _importData.LastName, _importData.DateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    new FakeStreamEvent(EmployeeId, new Events.Import.ImportedFromCvPartner(CompanyId, CompanyName, EmployeeId, _importData.FirstName, _importData.MiddleName, _importData.LastName, _importData.DateOfBirth,_importData.Email, _importData.Phone, _importData.Title, _importData.UpdatedAt, _importData.KeyQualifications, _importData.Technologies, _importData.Photo,DateTime.UtcNow, new Person(AdminId, Domain.Services.NameService.GetName(AdminFirstName, AdminLastName)), "IMPORT1"))
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
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId, NameService.GetName(_importData.FirstName, _importData.MiddleName, _importData.LastName), new Login(Constants.GoogleIdProvider, _importData.Email, string.Empty),DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                    
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
            _fakeGlobalRepository = new FakeRepository<Aggregates.Global>(GivenGlobal());
            _fakeImporter = new FakeCvPartnerImporter(new List<CvPartnerImportData>(new[] { _importData }));
            return new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeGlobalRepository, _fakeImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
