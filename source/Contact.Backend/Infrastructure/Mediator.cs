using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using NUnit.Framework;

namespace Contact.Backend.Infrastructure
{
    public class Mediator : IMediator
    {
        Dictionary<Tuple<Type, Type>,dynamic> subscribed = new Dictionary<Tuple<Type, Type>, dynamic>();
       
        public void Subscribe<TFrom, TTo>(Func<TFrom, IUser, TTo> handler)
        {
            subscribed[new Tuple<Type,Type>(typeof(TFrom),typeof(TTo))]  = handler;
        }

        public TTo Send<TFrom, TTo>(TFrom @from, IUser user)
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
        class UserMock : IUser
        {
            public string Id { get; private set; }
            public string UserName { get; set; }
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