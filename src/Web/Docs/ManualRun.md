# Run backend (VS2022) and frontend (VS Code) manually — ready-to-copy instructions

Summary
- Backend: Razor Pages app (`CA1.Web`) — run from Visual Studio 2022.
- Frontend: Angular SPA (`src/Web/ClientApp`) — run from VS Code.
- Ports used by this project:
  - Backend: `https://localhost:5001` and `http://localhost:5000`
  - SPA dev server (project expects): `https://localhost:44447`
- The SPA dev server proxies API calls to the backend using `src/Web/ClientApp/proxy.conf.js`. That proxy reads environment variables `ASPNETCORE_HTTPS_PORT` and `ASPNETCORE_URLS`.

Prerequisites
- Install .NET 10 SDK.
- Install Node.js and npm.
- Visual Studio 2022 (for backend) and VS Code (for client).

1) Prevent Visual Studio from auto-launching the SPA
- Ensure the debug profile does not set the hosting startup that auto-launches the SPA proxy.
- Confirm `ASPNETCORE_HOSTINGSTARTUPASSEMBLIES` is not present (or is empty) in `src/Web/Properties/launchSettings.json`.
- Example minimal `CA1.Web` profile (use this or remove the hosting startup entry if present):


{ "profiles": { "CA1.Web": { "commandName": "Project", "launchBrowser": true, "applicationUrl": "https://localhost:5001;http://localhost:5000", "environmentVariables": { "ASPNETCORE_ENVIRONMENT": "Development" } } } }

- If you previously had `"ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.SpaProxy"`, remove that entry so VS will not auto-start the SPA.

2) Start the backend in Visual Studio 2022
- Open the solution and set `CA1.Web` as the startup project.
- (Optional) Verify under __Project Properties > Debug__ that the Application URL is `https://localhost:5001;http://localhost:5000`.
- Start the backend using __Debug > Start Debugging__ (F5) or __Debug > Start Without Debugging__ (Ctrl+F5).
- Verify the backend is running: open `https://localhost:5001/health` or browse `https://localhost:5001`.

3) Start the Angular client in VS Code
- Open `src/Web/ClientApp` folder in VS Code.
- Install deps (once or when dependencies change):

cd src/Web/ClientApp npm install

- Start the dev server so it serves the SPA on the expected SPA port and proxies API requests to the backend.

PowerShell (recommended)

from src/Web/ClientApp
$Env:ASPNETCORE_HTTPS_PORT = "5001" $Env:ASPNETCORE_URLS = "https://localhost:5001;http://localhost:5000"

start Angular dev server on port 44447 with SSL and explicit proxy config
npm start -- --port 44447 --ssl true --proxy-config proxy.conf.js

cd src\Web\ClientApp set ASPNETCORE_HTTPS_PORT=5001 set ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000 npm run start -- --port 44447 --ssl true --proxy-config proxy.conf.js


- Open the SPA at `https://localhost:44447`. The Angular dev server will forward API requests (`/api`, `/Identity`, etc.) to the backend at `https://localhost:5001` per `proxy.conf.js`.

4) Alternate: serve built SPA from backend (single-port, production-like)
- Build SPA and let backend serve static files:

from repository root or src/Web
cd src/Web/ClientApp npm install npm run build -- --configuration production
copy built output to backend's wwwroot (or run publish which does this automatically)
then run backend normally (dotnet run or via VS)

- Or use `dotnet publish -c Release` from `src/Web` — `PublishRunWebpack` target in the project will build the SPA and include `dist/` in the published output.

5) Optional — VS Code task to start the client
- Create `src/Web/ClientApp/.vscode/tasks.json` to start the client with environment variables from VS Code:

src/Web/ClientApp/.vscode/tasks.json { "version": "2.0.0", "tasks": [ { "label": "Start Angular (proxy -> backend)", "type": "shell", "presentation": { "reveal": "always", "panel": "shared" }, "command": "npm", "args": ["run", "start", "--", "--port", "44447", "--ssl", "true", "--proxy-config", "proxy.conf.js"], "options": { "env": { "ASPNETCORE_HTTPS_PORT": "5001", "ASPNETCORE_URLS": "https://localhost:5001;http://localhost:5000" }, "cwd": "${workspaceFolder}/src/Web/ClientApp" }, "isBackground": true, "problemMatcher": [] } ] }


6) Troubleshooting & notes
- If the browser warns about certificates, trust the dev certificate for the backend or run the SPA without `--ssl true` (use `http://localhost:44447`), but avoid mixed-content (https page calling http API).
- If you see "Node.js is required..." run `npm install` in `ClientApp` or install Node.js system-wide.
- If proxying fails, confirm `src/Web/ClientApp/proxy.conf.js` resolves the backend target using `ASPNETCORE_HTTPS_PORT` or `ASPNETCORE_URLS`.
- To skip NSwag generation during Debug builds: pass MSBuild property `-p:SkipNSwag=true` when building.

Wrap-up
- Start the backend from Visual Studio (listening on `https://localhost:5001`).
- Start the SPA manually from VS Code on `https://localhost:44447` with environment variables set so the Angular dev server proxies API calls to the backend.
- If you want, I can paste a matching `launch.json` for VS Code or create the `tasks.json` file for you.  