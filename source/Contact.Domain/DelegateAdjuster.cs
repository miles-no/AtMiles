using System;
using System.Linq.Expressions;

namespace Contact.Domain
{
    public class DelegateAdjuster
    {
        public static Action<TBaseT> CastArgument<TBaseT, TDerivedT>(Expression<Action<TDerivedT>> source) where TDerivedT : TBaseT
        {
            if (typeof(TDerivedT) == typeof(TBaseT))
            {
                return (Action<TBaseT>)((Delegate)source.Compile());

            }
            var sourceParameter = Expression.Parameter(typeof(TBaseT), "source");
            var result = Expression.Lambda<Action<TBaseT>>(
                Expression.Invoke(
                    source,
                    Expression.Convert(sourceParameter, typeof(TDerivedT))),
                sourceParameter);
            return result.Compile();
        }
    }
}
