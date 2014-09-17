//using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;

namespace Contact.Domain.Test
{
    public abstract class EventSpecification<TCommand>
        where TCommand : Command
    {
        public Exception CaughtException;
        public abstract IEnumerable<Event> Produced();
        public abstract IEnumerable<FakeStreamEvent> Given();
        public abstract TCommand When();
        public abstract Handles<TCommand> OnHandler();
        public abstract IEnumerable<Event> Expect();
        public Exception ExpectedException;
        private Guid _playgroundId = Guid.Empty;

        public async void Setup()
        {
            CaughtException = null;
            var handler = OnHandler();

            try
            {
                await handler.Handle(When());
                var produced = Produced().ToList();
                var expected = Expect().ToList();
                if (expected.Count > 0 || produced.Count > 0)
                {
                    CompareEvents(expected, produced);
                }
                else
                {
                    CaughtException.ShouldBeEquivalentTo(ExpectedException);
                }
            }
            catch (Exception exception)
            {
                /*
                if (exception is UnitTestAssertException)
                {
                    Console.WriteLine(exception.Message);
                    throw;
                }
                */
                CaughtException = exception;
                CompareExceptions(CaughtException, ExpectedException);
            }
        }

        private static void CompareExceptions(Exception produced, Exception expected)
        {
            Assert.IsNotNull(produced);
            Assert.IsNotNull(expected);

            var expectedType = expected.GetType();
            var producedType = produced.GetType();

            Assert.AreEqual(expectedType, producedType, "Exception " + expectedType.Name + " not equal to " + producedType.Name);
            Assert.AreEqual(expected.Data.Count, produced.Data.Count);
            if (expected.Data.Count > 0)
            {
                foreach (var key in expected.Data.Keys)
                {
                    Assert.AreEqual(expected.Data[key], produced.Data[key]);
                }
            }
        }

        private static void CompareEvents(IReadOnlyList<Event> expected, IReadOnlyList<Event> produced)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(produced);
            Assert.AreEqual(expected.Count, produced.Count, "Expected events(" + expected.Count +
                            ") not equal to Produced events(" + produced.Count + ")");

            for (int i = 0; i < expected.Count; i++)
            {
                var expectedEvent = expected[i];
                var producedEvent = produced[i];
                CompareEvent(expectedEvent, producedEvent);
            }
        }

        private static void CompareEvent(Event expected, Event produced)
        {
            Assert.IsNotNull(expected);
            Assert.IsNotNull(produced);

            Type expectedType = expected.GetType();
            Type producedType = produced.GetType();

            Assert.AreEqual(expectedType, producedType, "Event " + expectedType.Name + " not equal to " + producedType.Name);

            var fields = expectedType.GetFields();

            foreach (var field in fields)
            {
                var expectedValue = field.GetValue(expected);
                var producedValue = field.GetValue(produced);
                var comp = new CompareObjects();
                Assert.IsTrue(comp.Compare(expectedValue, producedValue), "Different " + field.Name + " in the " + producedType.Name + " events");
            }
        }
    }
}
