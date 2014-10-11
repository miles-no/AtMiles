using System;
using System.Collections.Generic;
using no.miles.at.Backend.Domain.CommandHandlers;
using no.miles.at.Backend.Domain.Commands;
using no.miles.at.Backend.Domain.Events.Company;
using no.miles.at.Backend.Domain.Events.Employee;
using no.miles.at.Backend.Domain.Exceptions;
using no.miles.at.Backend.Domain.Services;
using no.miles.at.Backend.Domain.ValueTypes;
using NUnit.Framework;

namespace no.miles.at.Backend.Domain.Test.Employee.RemoveBusyTimeTests
{
    [TestFixture]
    [Category("BDD: Domain")]
    public class RemoveBusyTimeTestNotSelf : EventSpecification<RemoveBusyTime>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string EmployeeId = "id1";
        private const string EmployeeFirstName = "Ole";
        private const string EmployeeLastName = "Jensen";

        private const string EmployeeId2 = "id2";
        private const string EmployeeFirstName2 = "Jens";
        private const string EmployeeLastName2 = "Olsen";

        private static readonly DateTime Start1 = new DateTime(2014, 01, 01);
        private static readonly DateTime End1 = new DateTime(2015, 01, 01);
        private const short Percentage1 = 100;
        private const string Comment1 = "Client A";
        private const string BusyTimeId1 = "BT01";

        [Test]
        public async void remove_busy_time_not_on_self()
        {
            ExpectedException = new NoAccessException("Can only add busy-time to self");
            await Setup();
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
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeId,NameService.GetName(EmployeeFirstName, EmployeeLastName),null,DateTime.UtcNow,system, "INIT")),
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeId2,NameService.GetName(EmployeeFirstName2, EmployeeLastName2),null,DateTime.UtcNow,system, "INIT"))
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, EmployeeId, null, EmployeeFirstName, string.Empty, EmployeeLastName, DateTime.UtcNow,new Person(Constants.SystemUserId, Constants.SystemUserId), "INIT")),
                    new FakeStreamEvent(EmployeeId2, new EmployeeCreated(CompanyId, CompanyName, EmployeeId2, null, EmployeeFirstName2, string.Empty, EmployeeLastName2, DateTime.UtcNow,new Person(Constants.SystemUserId, Constants.SystemUserId), "INIT")),
                    new FakeStreamEvent(EmployeeId2, new BusyTimeAdded(CompanyId, CompanyName, EmployeeId2, NameService.GetName(EmployeeFirstName2, EmployeeLastName2), BusyTimeId1, Start1, End1, Percentage1, Comment1, DateTime.UtcNow, new Person(EmployeeId2, NameService.GetName(EmployeeFirstName2, EmployeeLastName2)),"FIRST"))
                };
            return events;
        }

        public override RemoveBusyTime When()
        {
            return new RemoveBusyTime(CompanyId, EmployeeId2, BusyTimeId1, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId, Constants.IgnoreVersion);
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
