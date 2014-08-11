using System;
using Microsoft.AspNet.Identity;

namespace Contact.Backend.Infrastructure
{
    public interface IMediator
    {
        void Subscribe<TFrom, TTo>(Func<TFrom, IUser, TTo> handler);
        TTo Send<TFrom, TTo>(TFrom from, IUser user);
    }
    
}