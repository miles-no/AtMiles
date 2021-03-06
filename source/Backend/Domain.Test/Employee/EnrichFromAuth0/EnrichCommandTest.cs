﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Import.Auth0;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Employee.EnrichFromAuth0
{
    [TestFixture]
    public class EnrichCommandTest
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string AdminId = "adm1";
        private const string AdminFirstName = "Admin";
        private const string AdminLastName = "Adminson";

        private readonly string _employeeGlobalId = new Guid().ToString();
        private readonly Login _employeeLoginId = new Login(Constants.GoogleIdProvider, "mail@miles.no");
        private const string EmployeeFirstName = "Kurt";
        private const string EmployeeLastName = "Kurtson";

        [SetUp]
        public void Init()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
        }

        //[Test]
        public async Task Test()
        {
            var handler = new GlobalCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository, null, null,
                new GetUsersFromAuth0("audience",
                    "secret", "https://atmiles.auth0.com"));

            await handler.Handle(new Commands.EnrichFromAuth0("miles", DateTime.UtcNow, new Person(AdminId, AdminFirstName),
                _correlationId, -5));
        }

        private IEnumerable<FakeStreamEvent> GivenCompany()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                    new FakeStreamEvent(CompanyId, new CompanyAdminAdded(CompanyId, CompanyName, AdminId,AdminFirstName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)),_correlationId)),
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(AdminId, new EmployeeCreated(CompanyId, CompanyName, AdminId, null, AdminFirstName, string.Empty, AdminLastName, DateTime.UtcNow, new Person(AdminId, NameService.GetName(AdminFirstName, AdminLastName)), _correlationId)),
                };
            return events;
        }

         
    }
}