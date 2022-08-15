namespace AzureSearch.FluentQuery.Visitors;

using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Constants;

public class AzureSearchVisitor : ExpressionVisitor
{
    private readonly StringBuilder _queryBuilder = new StringBuilder();
    private readonly Expression _expression;

    public AzureSearchVisitor(Expression expression)
        => _expression = expression;

    public AzureSearchVisitor()
        => _expression = Expression.Empty();

    public string Build(Expression expression)
    {
        _queryBuilder.Clear();

        Visit(expression);

        var translatedQuery = _queryBuilder.ToString();

        _queryBuilder.Clear();

        return translatedQuery;
    }

    public string Build()
    {
        _queryBuilder.Clear();

        Visit(_expression);

        var translatedQuery = _queryBuilder.ToString();
        _queryBuilder.Clear();

        return translatedQuery;
    }

    private void Out(string? value)
        => _queryBuilder.Append(value);


    private void Nested(Action nestedAction)
    {
        Out(Separators.OpenParenthesis);
        nestedAction();
        Out(Separators.CloseParenthesis);
    }

    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
        {
            Nested(() => Visit(node.Left));
        }
        else
        {
            Visit(node.Left);
        }

        var @operator = node.NodeType switch
        {
            ExpressionType.Equal => AzureSearchSyntax.Equal,
            ExpressionType.NotEqual => AzureSearchSyntax.NotEqual,
            ExpressionType.AndAlso => AzureSearchSyntax.And,
            ExpressionType.OrElse => AzureSearchSyntax.Or,
            ExpressionType.GreaterThan => AzureSearchSyntax.GreaterThan,
            ExpressionType.GreaterThanOrEqual => AzureSearchSyntax.GreaterThanOrEqual,
            ExpressionType.LessThan => AzureSearchSyntax.LessThan,
            ExpressionType.LessThanOrEqual => AzureSearchSyntax.LessThanOrEqual,
            _ => string.Empty
        };

        Out(Separators.Space);
        Out(@operator);
        Out(Separators.Space);

        if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
        {
            Nested(() => Visit(node.Right));
        }
        else
        {
            Visit(node.Right);
        }

        return node;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        Out(node.Name);
        return node;
    }

    protected override Expression VisitMember(MemberExpression node)
    {
        if (node.Expression is ConstantExpression constantExpression)
        {
            object container = constantExpression!.Value!;
            var value = node.Member switch
            {
                FieldInfo fieldInfo => fieldInfo!.GetValue(container)!,
                PropertyInfo propertyInfo => propertyInfo!.GetValue(container, null)!,
                _ => throw new ArgumentOutOfRangeException()
            };

            Visit(Expression.Constant(value));
        }
        else if (node.Expression is null && node.Member is MemberInfo memberInfo)
        {
            var compiledObject = Expression.Lambda(node).Compile();
            var value = compiledObject switch
            {
                Func<DateTime> dateTimeFunc => dateTimeFunc() as object,
                Func<DateTimeOffset> dateTimeOffsetFunc => dateTimeOffsetFunc() as object,
                _ => throw new ArgumentOutOfRangeException()
            };

            Visit(Expression.Constant(value));
        }
        else
        {
            Out(node.Member.Name!);
        }

        return node;
    }

    protected override Expression VisitConstant(ConstantExpression node)
    {
        Out(node.Value switch
        {
            string stringValue => $"'{stringValue}'",
            int intValue => $"{intValue}",
            double doubleValue => $"{doubleValue}",
            long longValue => $"{longValue}",
            bool boolValue => $"{boolValue}".ToLower(),
            DateTime dateTime => dateTime.ToString("O"),
            DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
            _ => throw new ArgumentOutOfRangeException()
        });

        return node;
    }

    protected override Expression VisitUnary(UnaryExpression node)
    {
        switch (node.NodeType)
        {
            case ExpressionType.Not:
                Out(AzureSearchSyntax.Not);
                Out(Separators.Space);
                Out(Separators.OpenParenthesis);
                break;
            case ExpressionType.Convert:
                break;
            default:
                throw new InvalidOperationException();
        }

        Visit(node.Operand);

        switch (node.NodeType)
        {
            case ExpressionType.Not:
                Out(Separators.CloseParenthesis);
                break;
            case ExpressionType.Convert:
                break;
            default:
                throw new InvalidOperationException();
        }

        return node;
    }

    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        var containsMultipleParameters = node.Parameters.Count > 1;
        if (containsMultipleParameters)
        {
            Out(Separators.OpenParenthesis);
        }

        for (int i = 0, n = node.Parameters.Count; i < n; i++)
        {
            if (i > 0)
            {
                Out(Separators.Comma);
                Out(Separators.Space);
            }

            Visit(node.Parameters[i]);
        }

        if (containsMultipleParameters)
        {
            Out(Separators.CloseParenthesis);
        }

        Out(Separators.Space);
        Out(Separators.ArrowFunction);
        Out(Separators.Space);

        Visit(node.Body);

        return node;
    }

    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node.NodeType == ExpressionType.Call)
        {
            Visit(node.Object);

            var equalsMethodCall = node.Method.Name.Equals(nameof(Equals));
            if (equalsMethodCall)
            {
                Out(Separators.Space);

                Out(node.Method.Name switch
                {
                    nameof(Equals) => AzureSearchSyntax.Equal,
                    _ => throw new ArgumentOutOfRangeException()
                });

                Out(Separators.Space);
            }
        }

        var firstArgumentExpression = node.Arguments?.FirstOrDefault();
        if (firstArgumentExpression is not null)
        {
            Visit(firstArgumentExpression);
        }

        if (node.Method.DeclaringType == typeof(Enumerable))
        {
            Out(Separators.Slash);
            Out(node.Method.Name switch
            {
                nameof(System.Linq.Enumerable.Any) => AzureSearchSyntax.Any,
                nameof(System.Linq.Enumerable.All) => AzureSearchSyntax.All,
                _ => throw new ArgumentOutOfRangeException()
            });
        }

        if (node.Arguments?.Any() ?? false)
        {
            var singleArgument = node.Arguments.Count == 1;

            if (!singleArgument)
            {
                Out(Separators.OpenParenthesis);
            }

            for (int i = 1, n = node.Arguments.Count; i < n; i++)
            {
                if (i > 1)
                {
                    Out(Separators.Comma);
                    Out(Separators.Space);

                }

                Visit(node.Arguments[i]);
            }

            if (!singleArgument)
            {
                Out(Separators.CloseParenthesis);
            }
        }

        return node;
    }
}
