using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.AddOfficeAdminTests
{
    [TestFixture]
    public class AddOfficeAdminUnknownFirstAdminTest : EventSpecification<AddOfficeAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;
        private FakeCvPartnerImporter _fakeCvPartnerImporter;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string Admin1Id = "adm1";
        private const string Admin1FirstName = "Admin";
        private const string Admin1LastName = "Adminson";

        private const string Admin2Id = "adm2";
        private const string Admin2FirstName = "Adminsine";
        private const string Admin2LastName = "Adminsen";
        private static readonly DateTime Admin2DateOfBirth = new DateTime(1979, 01, 01);

        [Test]
        public void add_office_admin_unknown_first_admin()
        {
            ExpectedException = new UnknownItemException(string.Empty);
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events = _fakeCompanyRepository.GetThenEvents();
            return events;
        }

        public override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            return all;
        }

        public IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, Admin1Id, NameService.GetName(Admin1FirstName , Admin1LastName), null, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)),_correlationId)),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Admin2Id, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, Admin2Id, null, Admin2FirstName, string.Empty, Admin2LastName, Admin2DateOfBirth, string.Empty,string.Empty,string.Empty,null,null, null,DateTime.UtcNow,new Person(Admin2Id, NameService.GetName(Admin2FirstName, Admin2LastName)), _correlationId)),
                };
            return events;
        }

        public override AddOfficeAdmin When()
        {
            var cmd = new AddOfficeAdmin(CompanyId, OfficeId, Admin2Id, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)), _correlationId, 5);
            return cmd;
        }

        public override Handles<AddOfficeAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            _fakeCvPartnerImporter = new FakeCvPartnerImporter();
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, _fakeCvPartnerImporter);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
