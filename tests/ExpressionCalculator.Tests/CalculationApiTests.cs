using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ExpressionCalculator.Tests;

public sealed class CalculationApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CalculationApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Theory]
    [InlineData("4+5*2", "14")]
    [InlineData("4+5/2", "6.5")]
    [InlineData("4+5/2-1", "5.5")]
    public async Task Evaluate_ValidRequest_ReturnsExpectedJson(string expression, string expected)
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/calculations/evaluate", new { expression });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture),
            document.RootElement.GetProperty("result").GetDecimal());
    }

    [Theory]
    [InlineData("")]
    [InlineData("4+")]
    [InlineData("1/0")]
    [InlineData("2*(3+4)")]
    public async Task Evaluate_InvalidRequest_ReturnsProblemDetails(string expression)
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/calculations/evaluate", new { expression });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.True(document.RootElement.TryGetProperty("title", out _));
    }

    [Fact]
    public async Task Evaluate_TooLongRequest_ReturnsBadRequest()
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/calculations/evaluate", new { expression = new string('1', 1001) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Evaluate_MissingExpression_ReturnsBadRequest()
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/calculations/evaluate", new { differentProperty = "4+5" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Evaluate_NullExpression_ReturnsBadRequestProblemDetails()
    {
        using var response = await _client.PostAsJsonAsync(
            "/api/calculations/evaluate", new { expression = (string?)null });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

}
