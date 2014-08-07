using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company
{
    [TestFixture]
    public class CloseOfficeWithEmployeesTest : EventSpecification<CloseOffice>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string companyId = "miles";
        private const string companyName = "Miles";

        private const string existingOfficeId = "bgn";
        private const string existingOfficeName = "Bergen";

        private readonly string officeId = Guid.NewGuid().ToString();
        private const string officeName = "Stavanger";


        private const string adminId = "adm1";
        private const string adminFirstName = "Admin";
        private const string adminLastName = "Adminson";
        private static readonly DateTime adminDateOfBirth = new DateTime(1980, 01, 01);

        [Test]
        public void close_office_with_employees()
        {
            ExpectedException = new ExistingChildItemsException();
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
                    new FakeStreamEvent(companyId, new OfficeOpened(companyId, companyName, existingOfficeId, existingOfficeName, null)),
                    new FakeStreamEvent(companyId, new CompanyAdminAdded(companyId, companyName, adminId, NameService.GetName(adminFirstName , adminLastName))),
                    new FakeStreamEvent(companyId, new OfficeOpened(companyId, companyName, officeId, officeName, null)),
                    new FakeStreamEvent(companyId, new EmployeeAdded(companyId, companyName, officeId, officeName, adminId, NameService.GetName(adminFirstName , adminLastName))),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(adminId, new EmployeeCreated(companyId, companyName, existingOfficeId, existingOfficeName, adminId, adminFirstName, adminLastName, adminDateOfBirth)),
                };
            return events;
        }

        public override CloseOffice When()
        {
            var cmd = new CloseOffice(companyId, officeId)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(2)
                .WithCreatedBy(new Person(adminId, NameService.GetName(adminFirstName, adminLastName)));
            return (CloseOffice)cmd;
        }

        public override Handles<CloseOffice> OnHandler()
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
