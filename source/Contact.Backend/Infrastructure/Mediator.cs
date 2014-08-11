using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.AspNet.Identity;
using NUnit.Framework;

namespace Contact.Backend.Infrastructure
{
    public class Mediator : IMediator
    {
        readonly Dictionary<Tuple<Type, Type>,dynamic> subscribed = new Dictionary<Tuple<Type, Type>, dynamic>();

        public void Subscribe<TFrom, TTo>(Func<TFrom, IIdentity, TTo> handler)
        {
            subscribed[new Tuple<Type,Type>(typeof(TFrom),typeof(TTo))]  = handler;
        }

        public TTo Send<TFrom, TTo>(TFrom @from, IIdentity user)
        {
            var test = new Tuple<Type, Type>(typeof (TFrom), typeof (TTo));
            if (subscribed.ContainsKey(test) == false)
            {
                throw new NotSupportedException("No subscription for " + typeof(TFrom) + "-" + typeof(TTo));
            }
            else
            {
                return subscribed[test](from, user);
            }
        }
    }

    public class MediatorTest
    {
        class UserMock : IIdentity
        {
            public string Name { get; private set; }
            public string AuthenticationType { get; private set; }
            public bool IsAuthenticated { get; private set; }
        }
        [Test]
        public void SubscribeTest()
        {
            var m = new Mediator();
            m.Subscribe<int, string>((number, user) => number.ToString());

            var res = m.Send<int, string>(789,new UserMock());
            
            Assert.AreEqual("789", res);
        }
    }
}