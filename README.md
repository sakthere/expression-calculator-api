# Expression Calculator API

ASP.NET Core API that evaluates arithmetic expressions containing non-negative integer operands and the operators `+`, `-`, `*`, and `/`.

The calculation is implemented by a custom parser in this repository; no expression-evaluation library is used.

## Approach

The API keeps HTTP concerns separate from expression evaluation. The controller accepts and validates the request, while `ExpressionEvaluator` parses and calculates the expression.

The evaluator uses a precedence-aware parser with two levels: multiplication and division are evaluated before addition and subtraction. Operators at the same level are processed from left to right. This keeps the implementation small while correctly handling expressions such as `4+5/2-1`.

`decimal` is used for calculations so division does not silently truncate. Invalid syntax, division by zero, and numeric overflow return controlled HTTP 400 responses.

## Tools and resources

- C# and ASP.NET Core on .NET 10
- xUnit and `Microsoft.AspNetCore.Mvc.Testing` for unit and API integration tests
- Visual Studio and the .NET CLI for development, build, and test execution
- Git and GitHub for source control and submission
- GitHub Copilot for AI-assisted development support
- Standard .NET and ASP.NET Core APIs only; no third-party expression-evaluation library

## Run

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet run --project src/ExpressionCalculator.Api --urls http://localhost:5000
```

## Use the API

`POST /api/calculations/evaluate`

```json
{
  "expression": "4+5/2-1"
}
```

Response:

```json
{
  "result": 5.5
}
```

Invalid input, including malformed expressions and division by zero, returns HTTP 400 with a Problem Details response.

## Assumptions

- Operands are non-negative integer literals.
- `*` and `/` take precedence over `+` and `-`.
- Operators at the same precedence are evaluated left-to-right.
- Whitespace is allowed between tokens.
- Parentheses, unary signs, and decimal input literals are not supported.
- Expressions are limited to 1,000 characters.
- Results use C# `decimal` so division does not silently truncate.

## Test

```bash
dotnet test
```

The test suite covers the supplied examples, precedence, associativity, malformed input, division by zero, numeric overflow, and the HTTP endpoint.

## Use of AI

GitHub Copilot was used to discuss the solution structure, review the custom parsing approach, identify useful test cases, and improve documentation. The implementation decisions were reviewed against the assignment requirements, and the solution was verified by running `dotnet restore`, a Release build, and the automated test suite. The final verification completed with 38 passing tests.
