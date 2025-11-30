using Azure.Identity;
using CA1.Application.Common.Interfaces;
using CA1.Infrastructure.Data;
using CA1.Web.Infrastructure;
using CA1.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
// Removed duplicate: using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using System;
using System.Linq;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<IUser, CurrentUser>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();

        builder.Services.AddRazorPages();

        // Customise default API behaviour
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "CA1 API";

            // Add an authenticate button to the Swagger UI in development
            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
    }

    public static void AddKeyVaultIfConfigured(this IHostApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
        if (!string.IsNullOrWhiteSpace(keyVaultUri))
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                new DefaultAzureCredential());
        }
    }
}
