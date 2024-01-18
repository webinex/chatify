using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Webinex.Chatify.DataAccess;

internal static class DbSetExtensions
{
    private static readonly ConcurrentDictionary<Type, object> KeySelectorCache = new();

    public static async Task<TEntity[]> FindManyNoTrackingAsync<TEntity, TKey>(
        this DbSet<TEntity> dbSet,
        IEnumerable<TKey> keys,
        bool required = true)
        where TEntity : class
    {
        keys = keys.Distinct().ToArray();
        var (expression, selector) = GetKeySelector<TEntity, TKey>(dbSet);

        var foundLocal = dbSet.Local
            .Where(x => keys.Contains(selector(x))).ToArray();

        if (foundLocal.Length == keys.Count())
            return foundLocal;

        var notFoundLocalKeys = keys.Except(foundLocal.Select(x => selector(x))).ToArray();
        var db = await dbSet.Where(CreateContainsExpression(expression, notFoundLocalKeys)).AsNoTracking().ToArrayAsync();
        var result = foundLocal.Concat(db).ToArray();

        if (!required || result.Length == keys.Count()) 
            return result;
        
        var notFoundKeys = keys.Except(result.Select(selector)).ToArray();
        throw new InvalidOperationException($"Not all entities found: {string.Join(", ", notFoundKeys)}");

    }

    private static Expression<Func<TEntity, bool>> CreateContainsExpression<TEntity, TKey>(
        Expression<Func<TEntity, TKey>> selector,
        IEnumerable<TKey> keys)
    {
        var containsExpression = CreateContainsExpression<TKey>();
        return Expression.Lambda<Func<TEntity, bool>>(
            Expression.Call(containsExpression.Method, Expression.Constant(keys), selector.Body),
            selector.Parameters);
    }

    private static MethodCallExpression CreateContainsExpression<TKey>()
    {
        Expression<Func<IEnumerable<TKey>, TKey, bool>> expression = (keys, key) => keys.Contains(key);
        return (MethodCallExpression)expression.Body;
    }

    private static Tuple<Expression<Func<TEntity, TKey>>, Func<TEntity, TKey>> GetKeySelector<TEntity, TKey>(
        DbSet<TEntity> dbSet)
        where TEntity : class
    {
        return (Tuple<Expression<Func<TEntity, TKey>>, Func<TEntity, TKey>>)KeySelectorCache.GetOrAdd(typeof(TEntity),
            _ => CreateKeySelector<TEntity, TKey>(dbSet));
    }

    private static Tuple<Expression<Func<TEntity, TKey>>, Func<TEntity, TKey>> CreateKeySelector<TEntity, TKey>(
        DbSet<TEntity> dbSet)
        where TEntity : class
    {
        var primaryKey = dbSet.EntityType.FindPrimaryKey() ?? throw new InvalidOperationException(
            $"Entity of type {typeof(TEntity)} doesn't have primary key definition");

        if (primaryKey.Properties.Count != 1)
            throw new InvalidOperationException(
                $"{nameof(FindManyNoTrackingAsync)} doesn't support composite primary keys. Entity type: {typeof(TEntity)}");

        var keyProperty = primaryKey.Properties[0];
        if (keyProperty.PropertyInfo!.PropertyType != typeof(TKey))
            throw new InvalidOperationException(
                $"Primary key type {keyProperty.PropertyInfo.PropertyType} doesn't match expected type {typeof(TKey)}");

        var expression = CreateKeySelector<TEntity, TKey>(keyProperty.Name);
        var func = expression.Compile();
        return Tuple.Create(expression, func);
    }

    private static Expression<Func<TEntity, TKey>> CreateKeySelector<TEntity, TKey>(string keyName)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "x");
        var property = Expression.Property(parameter, keyName);
        return Expression.Lambda<Func<TEntity, TKey>>(property, parameter);
    }
}
