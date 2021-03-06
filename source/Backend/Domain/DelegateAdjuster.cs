﻿using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace no.miles.at.Backend.Domain
{
    public static class DelegateAdjuster
    {
        public static Func<TBaseT, Task> CastArgument<TBaseT, TDerivedT>(Expression<Func<TDerivedT, Task>> source) where TDerivedT : TBaseT
        {
            if (typeof(TDerivedT) == typeof(TBaseT))
            {
                return (Func<TBaseT, Task>)((Delegate)source.Compile());

            }
            var sourceParameter = Expression.Parameter(typeof(TBaseT), "source");
            var result = Expression.Lambda<Func<TBaseT, Task>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(sourceParameter, typeof(TDerivedT))),
                sourceParameter);
            return result.Compile();
        }
    }
}
