﻿using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Company.RemoveCompanyAdminTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class RemoveCompanyAdminLastAdminTest : EventSpecification<RemoveCompanyAdmin>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string ExistingAdminId = "old1";
        private const string ExistingAdminFirstName = "Existing";
        private const string ExistingAdminLastName = "Admin";

        [Test]
        public async void remove_company_admin_last_admin()
        {
            ExpectedException = new LastItemException(string.Empty);
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            var events = _fakeCompanyRepository.GetThenEvents();
            return events;
        }

        protected override IEnumerable<FakeStreamEvent> Given()
        {
            var c = GivenCompany();
            var e = GivenEmployee();
            var all = new List<FakeStreamEvent>(c);
            all.AddRange(e);
            return all;
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName), DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)),_correlationId)),
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(ExistingAdminId, new EmployeeCreated(CompanyId, CompanyName, ExistingAdminId, null, ExistingAdminFirstName, string.Empty, ExistingAdminLastName, DateTime.UtcNow,new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)), _correlationId)),
                };
            return events;
        }

        protected override RemoveCompanyAdmin When()
        {
            var cmd = new RemoveCompanyAdmin(CompanyId, ExistingAdminId, DateTime.UtcNow, new Person(ExistingAdminId, NameService.GetName(ExistingAdminFirstName, ExistingAdminLastName)), _correlationId, 2);
            return cmd;
        }

        protected override Handles<RemoveCompanyAdmin> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        protected override IEnumerable<Event> Expect()
        {
            yield break;
        }
    }
}
