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

namespace no.miles.at.Backend.Domain.Test.Employee.UpdateBusyTimeTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class UpdateBusyTimeUnknownTest : EventSpecification<UpdateBusyTime>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string EmployeeId = "id1";
        private const string EmployeeFirstName = "Ole";
        private const string EmployeeLastName = "Jensen";

        private static readonly DateTime Start1 = new DateTime(2014, 01, 01);
        private static readonly DateTime Start2 = new DateTime(2014, 01, 02);
        private static readonly DateTime End1 = new DateTime(2015, 01, 01);
        private static readonly DateTime End2 = new DateTime(2015, 07, 01);
        private const short Percentage1 = 100;
        private const short Percentage2 = 80;
        private const string Comment1 = "Client A";
        private const string Comment2 = "Client A - Some extra";
        private const string BusyTimeId1 = "BT01";
        private const string BusyTimeId2 = "BT02";

        [Test]
        public async void update_busy_time_set_end_unknown()
        {
            ExpectedException = new UnknownItemException("Unknown ID for Busy time entry");
            await Setup();
        }

        protected override IEnumerable<Event> Produced()
        {
            return _fakeEmployeeRepository.GetThenEvents();
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
            var system = new Person(Constants.SystemUserId, Constants.SystemUserId);
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, system, "INIT")),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeId,NameService.GetName(EmployeeFirstName, EmployeeLastName),null,DateTime.UtcNow,system, "INIT"))
                };
            return events;
        }

        private IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, EmployeeId, null, EmployeeFirstName, string.Empty, EmployeeLastName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), "INIT")),
                    new FakeStreamEvent(EmployeeId, new BusyTimeAdded(CompanyId, CompanyName, EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName), BusyTimeId1, Start1, End1, Percentage1, Comment1, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)),"FIRST"))
                };
            return events;
        }

        protected override UpdateBusyTime When()
        {
            return new UpdateBusyTime(CompanyId, EmployeeId, BusyTimeId2, Start2, End2, Percentage2, Comment2, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId, Constants.IgnoreVersion);
        }

        protected override Handles<UpdateBusyTime> OnHandler()
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
