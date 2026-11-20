using System.ComponentModel.DataAnnotations;

namespace ExpressionCalculator.Api.Contracts;

/// <summary>
/// Request body for an expression evaluation.
/// </summary>
public sealed class EvaluateExpressionRequest
{
    [Required]
    [StringLength(1000)]
    public string Expression { get; init; } = string.Empty;
}
