var builder = DistributedApplication.CreateBuilder(args);

var webProject = builder.AddProject("Web", "../../src/Web/Web.csproj");
var serviceDefaultsProject = builder.AddProject("ca1-service-defaults", "../../Aspire/CA1.ServiceDefaults/CA1.ServiceDefaults.csproj");

webProject.WithReference(serviceDefaultsProject);

builder.Build().Run();
