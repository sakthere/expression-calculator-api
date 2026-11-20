# Expression Calculator API

ASP.NET Core API that evaluates arithmetic expressions containing non-negative integer operands and the operators `+`, `-`, `*`, and `/`.

The calculation is implemented by a custom parser in this repository; no expression-evaluation library is used.

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

## AI assistance

AI was used to assist with implementation, tests, documentation, and validation. The resulting code was reviewed and tested before submission.
