using System.Globalization;
using ExpressionCalculator.Api.Exceptions;

namespace ExpressionCalculator.Api.Services;

/// <summary>
/// Evaluates expressions with non-negative integer operands and basic arithmetic operators.
/// </summary>
public sealed class ExpressionEvaluator : IExpressionEvaluator
{
    public decimal Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
            throw new InvalidExpressionException("An expression is required.");

        if (expression.Length > 1000)
            throw new InvalidExpressionException("Expressions must not exceed 1000 characters.");

        var parser = new Parser(expression);
        return parser.Parse();
    }

    private sealed class Parser(string expression)
    {
        private int _position;

        public decimal Parse()
        {
            var result = ParseExpression();
            SkipWhitespace();

            if (_position < expression.Length)
                throw new InvalidExpressionException($"Unexpected character at position {_position + 1}.");

            return result;
        }

        // Addition and subtraction are evaluated after multiplication and division.
        private decimal ParseExpression()
        {
            var result = ParseTerm();

            while (true)
            {
                SkipWhitespace();
                if (_position >= expression.Length ||
                    (expression[_position] != '+' && expression[_position] != '-'))
                    return result;

                var operation = expression[_position++];
                var right = ParseTerm();

                try
                {
                    result = operation == '+' ? result + right : result - right;
                }
                catch (OverflowException)
                {
                    throw new InvalidExpressionException("The result exceeds the supported numeric range.");
                }
            }
        }

        // Multiplication and division are evaluated left to right.
        private decimal ParseTerm()
        {
            var result = ParseIntegerLiteral();

            while (true)
            {
                SkipWhitespace();
                if (_position >= expression.Length ||
                    (expression[_position] != '*' && expression[_position] != '/'))
                    return result;

                var operation = expression[_position++];
                var right = ParseIntegerLiteral();

                if (operation == '/' && right == 0m)
                    throw new InvalidExpressionException("Division by zero is not allowed.");

                try
                {
                    result = operation == '*' ? result * right : result / right;
                }
                catch (OverflowException)
                {
                    throw new InvalidExpressionException("The result exceeds the supported numeric range.");
                }
            }
        }

        // Operands must contain one or more digits.
        private decimal ParseIntegerLiteral()
        {
            SkipWhitespace();
            var start = _position;

            while (_position < expression.Length && expression[_position] is >= '0' and <= '9')
                _position++;

            if (start == _position)
                throw new InvalidExpressionException($"Expected a non-negative integer at position {_position + 1}.");

            var digits = expression.AsSpan(start, _position - start);
            if (!decimal.TryParse(digits, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
                throw new InvalidExpressionException("An integer operand exceeds the supported numeric range.");

            return value;
        }

        private void SkipWhitespace()
        {
            while (_position < expression.Length && char.IsWhiteSpace(expression[_position]))
                _position++;
        }
    }
}
