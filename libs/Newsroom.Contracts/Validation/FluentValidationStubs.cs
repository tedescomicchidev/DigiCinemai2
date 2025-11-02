using System.Collections.Generic;
using System.Linq;
namespace FluentValidation;

public abstract class AbstractValidator<T>
{
    private readonly List<IValidationRule<T>> _rules = new();

    protected IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(Func<T, TProperty> selector, string propertyName)
    {
        var rule = new PropertyRule<T, TProperty>(propertyName, selector);
        _rules.Add(rule);
        return rule;
    }

    protected IRuleBuilderInitial<T, TProperty> RuleFor<TProperty>(System.Linq.Expressions.Expression<Func<T, TProperty>> expression)
    {
        if (expression.Body is System.Linq.Expressions.MemberExpression member)
        {
            return RuleFor(expression.Compile(), member.Member.Name);
        }

        throw new ArgumentException("Only member access expressions are supported", nameof(expression));
    }

    public ValidationResult Validate(T instance)
    {
        var failures = new List<ValidationFailure>();
        foreach (var rule in _rules)
        {
            failures.AddRange(rule.Validate(instance));
        }

        return new ValidationResult(failures);
    }
}

public interface IRuleBuilderInitial<T, TProperty> : IRuleBuilder<T, TProperty>;

public interface IRuleBuilder<T, TProperty>
{
    IRuleBuilder<T, TProperty> NotEmpty(string? message = null);
    IRuleBuilder<T, TProperty> NotNull(string? message = null);
    IRuleBuilder<T, TProperty> MaximumLength(int length, string? message = null);
    IRuleBuilder<T, TProperty> MinimumLength(int length, string? message = null);
    IRuleBuilder<T, TProperty> GreaterThanOrEqualTo(int value, string? message = null);
    IRuleBuilder<T, TProperty> LessThanOrEqualTo(int value, string? message = null);
    IRuleBuilder<T, TProperty> Must(Func<TProperty, bool> predicate, string? message = null);
}

internal interface IValidationRule<T>
{
    IEnumerable<ValidationFailure> Validate(T instance);
}

public sealed record ValidationFailure(string PropertyName, string ErrorMessage);

public sealed record ValidationResult(IReadOnlyList<ValidationFailure> Errors)
{
    public bool IsValid => Errors.Count == 0;
}

internal sealed class PropertyRule<T, TProperty> : IRuleBuilderInitial<T, TProperty>, IValidationRule<T>
{
    private readonly string _propertyName;
    private readonly Func<T, TProperty> _selector;
    private readonly List<Func<TProperty, (bool Success, string? Message)>> _validators = new();

    public PropertyRule(string propertyName, Func<T, TProperty> selector)
    {
        _propertyName = propertyName;
        _selector = selector;
    }

    public IEnumerable<ValidationFailure> Validate(T instance)
    {
        var value = _selector(instance);
        foreach (var validator in _validators)
        {
            var (success, message) = validator(value);
            if (!success)
            {
                yield return new ValidationFailure(_propertyName, message ?? $"{_propertyName} is invalid");
            }
        }
    }

    public IRuleBuilder<T, TProperty> NotEmpty(string? message = null)
    {
        _validators.Add(value => value switch
        {
            null => (false, message ?? $"{_propertyName} must not be empty"),
            string s when string.IsNullOrWhiteSpace(s) => (false, message ?? $"{_propertyName} must not be empty"),
            IEnumerable<object?> e when !e.Cast<object?>().Any() => (false, message ?? $"{_propertyName} must not be empty"),
            _ => (true, null)
        });
        return this;
    }

    public IRuleBuilder<T, TProperty> NotNull(string? message = null)
    {
        _validators.Add(value => value is null ? (false, message ?? $"{_propertyName} must not be null") : (true, null));
        return this;
    }

    public IRuleBuilder<T, TProperty> MaximumLength(int length, string? message = null)
    {
        _validators.Add(value => value switch
        {
            string s when s.Length > length => (false, message ?? $"{_propertyName} must be {length} characters or fewer"),
            IEnumerable<object?> e when e.Cast<object?>().Count() > length => (false, message ?? $"{_propertyName} must contain {length} items or fewer"),
            _ => (true, null)
        });
        return this;
    }

    public IRuleBuilder<T, TProperty> MinimumLength(int length, string? message = null)
    {
        _validators.Add(value => value switch
        {
            string s when s.Length < length => (false, message ?? $"{_propertyName} must be at least {length} characters"),
            IEnumerable<object?> e when e.Cast<object?>().Count() < length => (false, message ?? $"{_propertyName} must contain at least {length} items"),
            _ => (true, null)
        });
        return this;
    }

    public IRuleBuilder<T, TProperty> GreaterThanOrEqualTo(int value, string? message = null)
    {
        _validators.Add(prop => prop is IComparable comparable && comparable.CompareTo(value) < 0
            ? (false, message ?? $"{_propertyName} must be >= {value}")
            : (true, null));
        return this;
    }

    public IRuleBuilder<T, TProperty> LessThanOrEqualTo(int value, string? message = null)
    {
        _validators.Add(prop => prop is IComparable comparable && comparable.CompareTo(value) > 0
            ? (false, message ?? $"{_propertyName} must be <= {value}")
            : (true, null));
        return this;
    }

    public IRuleBuilder<T, TProperty> Must(Func<TProperty, bool> predicate, string? message = null)
    {
        _validators.Add(value => predicate(value)
            ? (true, null)
            : (false, message ?? $"{_propertyName} does not satisfy the required condition"));
        return this;
    }
}
