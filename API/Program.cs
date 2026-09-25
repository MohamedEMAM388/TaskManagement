using System.Text.Json.Serialization;
using API.Middlewares;
using Application;
using Application.Common.Authorization;
using Infrastructure;
using Infrastructure.IdentityServices;
using Microsoft.AspNetCore.Authorization;

namespace API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        // Add services to the container
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddProblemDetails();


   
        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AnyAuthenticatedUser", 
                policy => policy.RequireRole("Admin", "User"))
            .AddPolicy("ResourceOwner",
                policy => policy.Requirements.Add(new ResourceOwnerRequirement()));
        
        builder.Services.AddScoped<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();

        var app = builder.Build();

        // seed roles 
        using var scope = app.Services.CreateScope();
        await IdentitySeeder.SeedRolesAsync(scope.ServiceProvider);

        // Global exception handling: converts unhandled exceptions
        // into standardized HTTP responses.
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // Controllers
        app.MapControllers();

        await app.RunAsync();
    }
}