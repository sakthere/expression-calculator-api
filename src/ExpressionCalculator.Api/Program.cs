using ExpressionCalculator.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<IExpressionEvaluator, ExpressionEvaluator>();

var app = builder.Build();

// Return a standard error response instead of exposing exception details.
app.UseExceptionHandler();
app.MapControllers();

app.Run();

// Used by integration tests to host the application in memory.
public partial class Program { }
