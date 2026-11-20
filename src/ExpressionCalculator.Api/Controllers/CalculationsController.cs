using ExpressionCalculator.Api.Contracts;
using ExpressionCalculator.Api.Exceptions;
using ExpressionCalculator.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpressionCalculator.Api.Controllers;

[ApiController]
[Route("api/calculations")]
public sealed class CalculationsController(IExpressionEvaluator evaluator) : ControllerBase
{
    [HttpPost("evaluate")]
    [ProducesResponseType(typeof(EvaluateExpressionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<EvaluateExpressionResponse> Evaluate([FromBody] EvaluateExpressionRequest request)
    {
        try
        {
            return Ok(new EvaluateExpressionResponse(evaluator.Evaluate(request.Expression)));
        }
        catch (InvalidExpressionException exception)
        {
            return Problem(
                title: "Invalid expression",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }
}
