using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events;
using Contact.Domain.Exceptions;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company
{
    public class AddCompanyAdminWithoutPermissionTest : EventSpecification<AddCompanyAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string companyId = "miles";
        private const string companyName = "Miles";

        private const string existingAdminId = "old1";
        private const string existingAdminFirstName = "Existing";
        private const string existingAdminLastName = "Admin";
        private static readonly DateTime existingAdminDateOfBirth = new DateTime(1980, 01, 01);

        private const string newAdminId = "new1";
        private const string newAdminFirstName = "New";
        private const string newAdminLastName = "Admin";
        private static readonly DateTime newAdminDateOfBirth = new DateTime(1981, 01, 01);

        private const string officeId = "office1";
        private const string officeName = "Stavanger";


        [Test]
        public void add_company_admin_without_permission()
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
                    
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(existingAdminId, new EmployeeCreated(companyId, companyName, officeId, officeName, existingAdminId, existingAdminFirstName, existingAdminLastName, existingAdminDateOfBirth)),
                    new FakeStreamEvent(newAdminId, new EmployeeCreated(companyId, companyName, officeId, officeName, newAdminId, newAdminFirstName, newAdminLastName, newAdminDateOfBirth)),
                };
            return events;
        }

        public override AddCompanyAdmin When()
        {
            var cmd = new AddCompanyAdmin(companyId, newAdminId)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(2)
                .WithCreatedBy(new Person(existingAdminId, existingAdminFirstName + " " + existingAdminLastName));
            return (AddCompanyAdmin)cmd;
        }

        public override Handles<AddCompanyAdmin> OnHandler()
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
