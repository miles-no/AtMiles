using System;
using System.Security.Principal;

namespace Contact.Backend.Infrastructure
{
    public interface IMediator
    {
        void Subscribe<TFrom, TTo>(Func<TFrom, IIdentity, TTo> handler);
        TTo Send<TFrom, TTo>(TFrom from, IIdentity user);
    }
    
}