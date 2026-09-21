using API.Middlewares;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.Negotiate;

namespace API;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers();
        // Add services to the container
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
            .AddNegotiate();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        var app = builder.Build();

        // Global exception handling: converts FluentValidation/NotFound/BusinessRule
        // exceptions thrown from handlers into proper HTTP responses (400/404/500).
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // Controllers
        app.MapControllers();

        app.Run();
    }
}