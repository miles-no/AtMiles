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
    public class AddOfficeAdminWithoutPermissionTest : EventSpecification<AddOfficeAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private readonly string _officeId = Guid.NewGuid().ToString();
        private const string OfficeName = "Stavanger";

        private const string Admin1Id = "adm1";
        private const string Admin1FirstName = "Admin";
        private const string Admin1LastName = "Adminson";
        private static readonly DateTime Admin1DateOfBirth = new DateTime(1980, 01, 01);

        private const string Admin2Id = "adm2";
        private const string Admin2FirstName = "Adminsine";
        private const string Admin2LastName = "Adminsen";
        private static readonly DateTime Admin2DateOfBirth = new DateTime(1979, 01, 01);

        [Test]
        public void add_office_admin_without_permission()
        {
            ExpectedException = new NoAccessException();
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
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName)),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, _officeId, OfficeName, null)),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Admin1Id, new EmployeeCreated(CompanyId, CompanyName, _officeId, OfficeName, Admin1Id, Admin1FirstName, Admin1LastName, Admin1DateOfBirth)),
                    new FakeStreamEvent(Admin2Id, new EmployeeCreated(CompanyId, CompanyName, _officeId, OfficeName, Admin2Id, Admin2FirstName, Admin2LastName, Admin2DateOfBirth)),
                };
            return events;
        }

        public override AddOfficeAdmin When()
        {
            var cmd = new AddOfficeAdmin(CompanyId, _officeId, Admin2Id)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(5)
                .WithCreatedBy(new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)));
            return (AddOfficeAdmin)cmd;
        }

        public override Handles<AddOfficeAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
