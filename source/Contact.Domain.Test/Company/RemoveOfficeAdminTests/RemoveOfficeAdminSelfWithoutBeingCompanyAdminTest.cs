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

namespace Contact.Domain.Test.Company.RemoveOfficeAdminTests
{
    [TestFixture]
    public class RemoveOfficeAdminSelfWithoutBeingCompanyAdminTest : EventSpecification<RemoveOfficeAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string Admin1Id = "adm1";
        private const string Admin1FirstName = "Admin";
        private const string Admin1LastName = "Adminson";
        private static readonly DateTime Admin1DateOfBirth = new DateTime(1980, 01, 01);

        [Test]
        public void remove_office_admin_self_without_being_company_admin()
        {
            ExpectedException = new NoAccessException(string.Empty);
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
                    new FakeStreamEvent(CompanyId, new OfficeAdminAdded(CompanyId, CompanyName, OfficeId, OfficeName, Admin1Id, NameService.GetName(Admin1FirstName , Admin1LastName), DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)),_correlationId)),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(Admin1Id, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, Admin1Id, null, Admin1FirstName, string.Empty, Admin1LastName, Admin1DateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)), _correlationId)),
                };
            return events;
        }

        public override RemoveOfficeAdmin When()
        {
            var cmd = new RemoveOfficeAdmin(CompanyId, OfficeId, Admin1Id, DateTime.UtcNow, new Person(Admin1Id, NameService.GetName(Admin1FirstName, Admin1LastName)), _correlationId, 5);
            return cmd;
        }

        public override Handles<RemoveOfficeAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
