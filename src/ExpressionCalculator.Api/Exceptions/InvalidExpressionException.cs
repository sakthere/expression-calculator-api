namespace ExpressionCalculator.Api.Exceptions;

public sealed class InvalidExpressionException : Exception
{
    public InvalidExpressionException(string message) : base(message) { }
}
