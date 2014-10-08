using System;
using System.Collections.Generic;
using System.Security.Principal;

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
            var subscription = new Tuple<Type, Type>(typeof (TFrom), typeof (TTo));
            if (subscribed.ContainsKey(subscription) == false)
            {
                throw new NotSupportedException("No subscription for " + typeof(TFrom) + "-" + typeof(TTo));
            }
            
            return subscribed[subscription](@from, user);
        }
    }
}