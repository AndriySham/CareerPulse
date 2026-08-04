using CareerPulse.Api.Middleware;
using CareerPulse.Application.Extensions;
using CareerPulse.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ───────────────────────────────────────────────────────────────

builder.Services.AddControllers();

// Application layer: MediatR + FluentValidation + ValidationBehavior
builder.Services.AddApplicationServices();

// Infrastructure layer: EF Core (PostgreSQL) + IFileStorage
builder.Services.AddInfrastructureServices(builder.Configuration);

// Swagger / OpenAPI — ADR 009
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CareerPulse API",
        Version = "v1",
        Description = "Personal Career CRM — Backend API"
    });
});

// ─── App Pipeline ────────────────────────────────────────────────────────────

var app = builder.Build();

// RFC 7807 global exception handler — must be first in pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CareerPulse API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
