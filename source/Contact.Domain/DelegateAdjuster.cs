using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contact.Domain
{
    public class DelegateAdjuster
    {
        public static Func<TBaseT, Task> CastArgument<TBaseT, TDerivedT>(Expression<Action<TDerivedT>> source) where TDerivedT : TBaseT
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
