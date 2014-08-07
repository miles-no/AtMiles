using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.RemoveOfficeAdminTests
{
    [TestFixture]
    public class RemoveOfficeAdminSelfWithoutBeingCompanyAdmin : EventSpecification<RemoveOfficeAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string companyId = "miles";
        private const string companyName = "Miles";

        private readonly string officeId = Guid.NewGuid().ToString();
        private const string officeName = "Stavanger";

        private const string admin1Id = "adm1";
        private const string admin1FirstName = "Admin";
        private const string admin1LastName = "Adminson";
        private static readonly DateTime admin1DateOfBirth = new DateTime(1980, 01, 01);

        [Test]
        public void remove_office_admin_self_without_being_company_admin()
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
                    new FakeStreamEvent(companyId, new CompanyCreated(companyId, companyName)),
                    new FakeStreamEvent(companyId, new OfficeOpened(companyId, companyName, officeId, officeName, null)),
                    new FakeStreamEvent(companyId, new EmployeeAdded(companyId, companyName, officeId, officeName, admin1Id, NameService.GetName(admin1FirstName , admin1LastName))),
                    new FakeStreamEvent(companyId, new OfficeAdminAdded(companyId, companyName, officeId, officeName, admin1Id, NameService.GetName(admin1FirstName , admin1LastName))),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(admin1Id, new EmployeeCreated(companyId, companyName, officeId, officeName, admin1Id, admin1FirstName, admin1LastName, admin1DateOfBirth)),
                };
            return events;
        }

        public override RemoveOfficeAdmin When()
        {
            var cmd = new RemoveOfficeAdmin(companyId, officeId, admin1Id)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(5)
                .WithCreatedBy(new Person(admin1Id, NameService.GetName(admin1FirstName, admin1LastName)));
            return (RemoveOfficeAdmin)cmd;
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
