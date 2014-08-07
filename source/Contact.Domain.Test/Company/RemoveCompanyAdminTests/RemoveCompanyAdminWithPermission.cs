﻿
using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Company.RemoveCompanyAdminTests
{
    [TestFixture]
    public class RemoveCompanyAdminWithPermission : EventSpecification<RemoveCompanyAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string ExistingAdminId = "old1";
        private const string ExistingAdminFirstName = "Existing";
        private const string ExistingAdminLastName = "Admin";
        private static readonly DateTime ExistingAdminDateOfBirth = new DateTime(1980, 01, 01);

        private const string NewAdminId = "new1";
        private const string NewAdminFirstName = "New";
        private const string NewAdminLastName = "Admin";
        private static readonly DateTime NewAdminDateOfBirth = new DateTime(1981, 01, 01);

        private const string OfficeId = "office1";
        private const string OfficeName = "Stavanger";

        [Test]
        public void remove_company_admin_with_permission()
        {
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
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, ExistingAdminId, ExistingAdminFirstName + " " + ExistingAdminLastName)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, NewAdminId, NameService.GetName(NewAdminFirstName, NewAdminLastName))),
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(ExistingAdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, ExistingAdminId, ExistingAdminFirstName, ExistingAdminLastName, ExistingAdminDateOfBirth)),
                    new FakeStreamEvent(NewAdminId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, NewAdminId, NewAdminFirstName, NewAdminLastName, NewAdminDateOfBirth)),
                };
            return events;
        }

        public override RemoveCompanyAdmin When()
        {
            var cmd = new RemoveCompanyAdmin(CompanyId, NewAdminId)
                .WithCreated(DateTime.UtcNow)
                .WithCorrelationId(_correlationId)
                .WithBasedOnVersion(2)
                .WithCreatedBy(new Person(ExistingAdminId, ExistingAdminFirstName + " " + ExistingAdminLastName));
            return (RemoveCompanyAdmin)cmd;
        }

        public override Handles<RemoveCompanyAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new CompanyAdminRemoved(CompanyId, CompanyName, NewAdminId, NameService.GetName(NewAdminFirstName, NewAdminLastName))
                                        .WithCorrelationId(_correlationId)
                };
            return events;
        }
    }
}
