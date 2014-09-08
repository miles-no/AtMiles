﻿using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Exceptions;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Employee.RemoveBusyTimeTests
{
    [TestFixture]
    public class RemoveBusyTimeUnknownTest : EventSpecification<RemoveBusyTime>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string OfficeId = "SVG";
        private const string OfficeName = "Stavanger";

        private const string EmployeeId = "id1";
        private const string EmployeeFirstName = "Ole";
        private const string EmployeeLastName = "Jensen";
        private static readonly DateTime EmployeeDateOfBirth = new DateTime(1980, 01, 01);

        private static readonly DateTime Start1 = new DateTime(2014, 01, 01);
        private static readonly DateTime End1 = new DateTime(2015, 01, 01);
        private const short Percentage1 = 100;
        private const string Comment1 = "Client A";
        private const string BusyTimeId1 = "BT01";

        [Test]
        public void remove_busy_time_unknown()
        {
            ExpectedException = new UnknownItemException("Unknown ID for Busy time entry");
            Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events = _fakeEmployeeRepository.GetThenEvents();
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
            var system = new Person(Constants.SystemUserId, Constants.SystemUserId);
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(CompanyId, new CompanyCreated(CompanyId, CompanyName, DateTime.UtcNow, system, "INIT")),
                    new FakeStreamEvent(CompanyId, new OfficeOpened(CompanyId, CompanyName, OfficeId, OfficeName, null, DateTime.UtcNow, system, "INIT")),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId,NameService.GetName(EmployeeFirstName, EmployeeLastName),null,DateTime.UtcNow,system, "INIT"))
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, OfficeId, OfficeName, EmployeeId, null, EmployeeFirstName, string.Empty, EmployeeLastName, EmployeeDateOfBirth, string.Empty,string.Empty,string.Empty, null, null,DateTime.UtcNow,new Person(Constants.SystemUserId, Constants.SystemUserId), "INIT"))
                };
            return events;
        }

        public override RemoveBusyTime When()
        {
            return new RemoveBusyTime(CompanyId, OfficeId, EmployeeId, BusyTimeId1, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<RemoveBusyTime> OnHandler()
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