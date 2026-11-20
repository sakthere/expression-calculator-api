namespace ExpressionCalculator.Api.Services;

public interface IExpressionEvaluator
{
    decimal Evaluate(string expression);
}
