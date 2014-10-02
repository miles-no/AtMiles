using System;
using System.Collections.Generic;
using Contact.Domain.CommandHandlers;
using Contact.Domain.Commands;
using Contact.Domain.Events.Company;
using Contact.Domain.Events.Employee;
using Contact.Domain.Services;
using Contact.Domain.ValueTypes;
using NUnit.Framework;

namespace Contact.Domain.Test.Employee.UpdateBusyTimeSetFinishDateTests
{
    [TestFixture]
    public class UpdateBusyTimeSetFinishDateTest : EventSpecification<UpdateBusyTimeSetEndDate>
    {
        private readonly string _correlationId = Guid.NewGuid().ToString();
        private DateTime _timestamp = DateTime.MinValue;
        private FakeRepository<Aggregates.Company> _fakeCompanyRepository;
        private FakeRepository<Aggregates.Employee> _fakeEmployeeRepository;

        private const string CompanyId = "miles";
        private const string CompanyName = "Miles";

        private const string EmployeeId = "id1";
        private const string EmployeeFirstName = "Ole";
        private const string EmployeeLastName = "Jensen";

        private static readonly DateTime Start1 = new DateTime(2014, 01, 01);
        private static readonly DateTime End1 = new DateTime(2015, 01, 01);
        private static readonly DateTime End2 = new DateTime(2015, 07, 01);
        private const short Percentage1 = 100;
        private const string Comment1 = "Client A";
        private const string BusyTimeId1 = "BT01";

        [Test]
        public async void update_busy_time_set_end()
        {
            await Setup();
        }

        public override IEnumerable<Event> Produced()
        {
            var events = _fakeEmployeeRepository.GetThenEvents();
            if (events.Count == 1)
            {
                _timestamp = events[0].Created;
            }
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
                    new FakeStreamEvent(CompanyId, new EmployeeAdded(CompanyId, CompanyName, EmployeeId,NameService.GetName(EmployeeFirstName, EmployeeLastName),null,DateTime.UtcNow,system, "INIT"))
                };
            return events;
        }

        public IEnumerable<FakeStreamEvent> GivenEmployee()
        {
            var events = new List<FakeStreamEvent>
                {
                    new FakeStreamEvent(EmployeeId, new EmployeeCreated(CompanyId, CompanyName, EmployeeId, null, EmployeeFirstName, string.Empty, EmployeeLastName, DateTime.UtcNow, new Person(Constants.SystemUserId, Constants.SystemUserId), "INIT")),
                    new FakeStreamEvent(EmployeeId, new BusyTimeAdded(CompanyId, CompanyName, EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName), BusyTimeId1, Start1, End1, Percentage1, Comment1, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)),"FIRST"))
                };
            return events;
        }

        public override UpdateBusyTimeSetEndDate When()
        {
            return new UpdateBusyTimeSetEndDate(CompanyId, EmployeeId, BusyTimeId1, End2, DateTime.UtcNow, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)), _correlationId, Constants.IgnoreVersion);
        }

        public override Handles<UpdateBusyTimeSetEndDate> OnHandler()
        {
            _fakeCompanyRepository = new FakeRepository<Aggregates.Company>(GivenCompany());
            _fakeEmployeeRepository = new FakeRepository<Aggregates.Employee>(GivenEmployee());
            return new CompanyCommandHandler(_fakeCompanyRepository, _fakeEmployeeRepository);
        }

        public override IEnumerable<Event> Expect()
        {
            var events = new List<Event>
                {
                    new BusyTimeUpdatedNewEndDate(CompanyId, CompanyName, EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName), BusyTimeId1, End2, _timestamp, new Person(EmployeeId, NameService.GetName(EmployeeFirstName, EmployeeLastName)),_correlationId)
                };
            return events;
        }
    }
}
