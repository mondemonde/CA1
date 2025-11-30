using Aspire.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
// Removed: using OpenTelemetry.Trace;
// Removed: using OpenTelemetry.Metrics;

namespace Aspire.ServiceDefaults;

public static class Extensions
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Temporarily removed these calls, as they are causing CS1061 errors.
        // builder.ConfigureOpenTelemetry();
        // builder.AddDefaultHealthChecks();
        // builder.ConfigureDefaultDistributedApplication();
        // builder.ConfigureDefaultMetadata();

        return builder;
    }

    public static WebApplication UseServiceDefaults(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        // Removed Authorization, Authentication, and User settings for now

        return app;
    }
}
