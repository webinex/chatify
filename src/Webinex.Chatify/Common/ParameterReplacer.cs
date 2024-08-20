using System.Linq.Expressions;

namespace Webinex.Chatify.Common;

internal static class ParameterReplacer
{
    internal static Expression Replace(
        Expression expression,
        ParameterExpression oldParameter,
        Expression newParameter)
    {
        return new Visitor(oldParameter, newParameter).Visit(expression)!;
    }

    private class Visitor
        : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public Visitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression? Visit(Expression? node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
