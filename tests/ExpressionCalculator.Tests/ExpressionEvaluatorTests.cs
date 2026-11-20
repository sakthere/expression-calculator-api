using System.Globalization;
using ExpressionCalculator.Api.Exceptions;
using ExpressionCalculator.Api.Services;
using Xunit;

namespace ExpressionCalculator.Tests;

public sealed class ExpressionEvaluatorTests
{
    private readonly IExpressionEvaluator _evaluator = new ExpressionEvaluator();

    [Theory]
    [InlineData("4+5*2", "14")]
    [InlineData("4+5/2", "6.5")]
    [InlineData("4+5/2-1", "5.5")]
    [InlineData("20/5*2", "8")]
    [InlineData("8-3-2", "3")]
    [InlineData("8-12", "-4")]
    [InlineData("12/4/3", "1")]
    [InlineData("2*3+4*5", "26")]
    [InlineData("  4  +\t5 * 2  ", "14")]
    [InlineData("0004+0002", "6")]
    [InlineData("0", "0")]
    [InlineData("1/3", "0.3333333333333333333333333333")]
    [InlineData("10-2*4+3", "5")]
    public void Evaluate_ValidExpression_ProducesExpectedResult(string expression, string expected)
    {
        var result = _evaluator.Evaluate(expression);
        Assert.Equal(decimal.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("4+")]
    [InlineData("4++5")]
    [InlineData("4.5+2")]
    [InlineData("-4+5")]
    [InlineData("1+-2")]
    [InlineData("1 2")]
    [InlineData("2*(3+4)")]
    [InlineData("hello")]
    [InlineData("10/0")]
    [InlineData("10/00")]
    [InlineData("9999999999999999999999999999999999999")]
    public void Evaluate_InvalidExpression_ThrowsControlledException(string expression)
    {
        Assert.Throws<InvalidExpressionException>(() => _evaluator.Evaluate(expression));
    }

    [Fact]
    public void Evaluate_OverlongExpression_ThrowsControlledException()
    {
        Assert.Throws<InvalidExpressionException>(() => _evaluator.Evaluate(new string('1', 1001)));
    }

    [Fact]
    public void Evaluate_DecimalOverflow_ThrowsControlledException()
    {
        Assert.Throws<InvalidExpressionException>(() =>
            _evaluator.Evaluate("79228162514264337593543950335+1"));
    }
}
