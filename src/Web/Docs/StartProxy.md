# Conversation — CA1.Web: how it runs & SPA proxy inspection

## Summary
- Backend: Razor Pages app (`src\Web\Program.cs`) targeting .NET 10.
- Frontend: Angular SPA in `ClientApp\`.
- SPA proxy is enabled via the `Microsoft.AspNetCore.SpaProxy` hosting startup (activated by environment variable) and project properties in `src\Web\Web.csproj`.
- Development ports (from `launchSettings.json`): `https://localhost:5001` and `http://localhost:5000`; IIS Express profile uses `http://localhost:61846` (SSL port `44312`).
- The SPA dev server is started with `npm start` in `ClientApp\` and the csproj sets `SpaProxyServerUrl` to `https://localhost:44447`.

## Prerequisites
- Install .NET 10 SDK.
- Install Node.js and npm.
- Optional: Visual Studio 2022.

## How to run — Visual Studio
1. Open the solution and set the web project (`CA1.Web`) as the startup project.
2. Start the app with __Debug > Start Debugging__ (F5) or __Debug > Start Without Debugging__ (Ctrl+F5).
3. What happens:
   - MSBuild runs the `DebugEnsureNodeEnv` target (if Debug) which checks for Node and runs `npm install` in `ClientApp\` when `node_modules` is missing.
   - The hosting startup `Microsoft.AspNetCore.SpaProxy` (enabled via `ASPNETCORE_HOSTINGSTARTUPASSEMBLIES`) runs the SPA dev server with the `SpaProxyLaunchCommand` and proxies SPA requests to the `SpaProxyServerUrl`.
   - Backend serves Razor Pages, exposes API endpoints under `/api`, and serves static files. `MapFallbackToFile("index.html")` returns the SPA for non-API routes.

## How to run — CLI
From `src\Web`: