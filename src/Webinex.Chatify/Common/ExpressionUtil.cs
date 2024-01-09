using System.Linq.Expressions;

namespace Webinex.Chatify.Common;

internal static class ExpressionUtil
{
    public static Expression<Func<T, bool>> OrElse<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var body = Expression.OrElse(
            Expression.Invoke(left, param),
            Expression.Invoke(right, param)
        );
        var lambda = Expression.Lambda<Func<T, bool>>(body, param);
        return lambda;
    }
}
