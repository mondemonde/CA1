using CA1.Infrastructure.Data;
using CA1.Web.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag; // Added for NSwag namespace
using NSwag.AspNetCore; // Included for NSwag ASP.NET Core integration
using Serilog; // For Serilog configuration
using Serilog.AspNetCore; // For builder.Host.UseSerilog extension method
using Aspire.ServiceDefaults; // For AddServiceDefaults and UseServiceDefaults extension methods

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration));

builder.AddServiceDefaults();
builder.AddKeyVaultIfConfigured();
builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();

var app = builder.Build();

app.UseServiceDefaults();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.InitialiseDatabaseAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.UseExceptionHandler();


app.MapEndpoints();

app.Run();

public partial class Program { }
