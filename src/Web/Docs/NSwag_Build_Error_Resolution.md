# NSwag Build Error Resolution

## Problem Description

During the build process, the following error occurred related to NSwag command line tool:

```
System.IO.FileNotFoundException: Could not load file or assembly 'NSwag.AspNetCore, Version=14.6.2.0, Culture=neutral, PublicKeyToken=c2d88086e098d109'. The system cannot find the file specified.
```

This initial error was followed by subsequent errors when attempting to fix it, specifically:

```
System.InvalidOperationException: The specified runtime in the document (Net80) differs from the current process runtime (Net100). Change the runtime with the '/runtime:Net80' parameter or run the file with the correct command line binary.
```

and

```
System.InvalidOperationException: Project outputs could not be located in 'I:\_CleanArchitecture\CA1\artifacts\bin\Web\debug_net80\'. Ensure that the project has been built.
```

## Cause

The root cause of these issues was a mismatch in the targeted .NET runtime versions. The `global.json` file indicated that the project was targeting .NET 10.0 (`"version": "10.0.100"`), yet initial attempts to resolve the `FileNotFoundException` involved incorrectly configuring NSwag to use `Net80`.

The `NSwag.MSBuild` task within `Web.csproj` was attempting to execute the NSwag command with a `Net100` toolchain, while the `config.nswag` file was configured for `Net80`, leading to the runtime mismatch. When attempting to force the `runtime:Net80` via command-line parameter, it was not recognized, and then when changing to `$(NSwagExe_Net80)` it resulted in outputs not found for `Net80`.

## Solution

The solution involved reverting the incorrect changes and ensuring that both the `config.nswag` file and the NSwag command in `Web.csproj` explicitly targeted `Net100`, aligning with the project's `.NET 10.0` SDK.

1.  **Reverted `src/Web/config.nswag`**:
    *   The top-level `runtime` property was set to `"Net100"`.
    *   The `runtime` property within the `aspNetCoreToOpenApi` section was removed (as it would inherit from the global setting).

    ```json
    {
      "runtime": "Net100",
      "defaultVariables": null,
      "documentGenerator": {
        "aspNetCoreToOpenApi": {
          "project": "Web.csproj",
          "documentName": "v1",
          "msBuildProjectExtensionsPath": null,
          "configuration": null,
          "runtime": null, // Reverted to null to inherit from global
          "targetFramework": null,
          // ... other properties
        }
      },
      // ... other sections
    }
    ```

2.  **Reverted `src/Web/Web.csproj`**:
    *   The `Command` attribute within the `NSwag` target was updated to use `$(NSwagExe_Net100)`, ensuring that the correct NSwag executable for .NET 10.0 was being invoked.

    ```xml
    <Target Name="NSwag" AfterTargets="PostBuildEvent" Condition=" '$(Configuration)' == 'Debug' And '$(SkipNSwag)' != 'True' ">
      <Exec ConsoleToMSBuild="true" ContinueOnError="true" WorkingDirectory="$(ProjectDir)" EnvironmentVariables="ASPNETCORE_ENVIRONMENT=Development" Command="$(NSwagExe_Net100) run config.nswag /variables:Configuration=$(Configuration)">
        <Output TaskParameter="ExitCode" PropertyName="NSwagExitCode" />
        <Output TaskParameter="ConsoleOutput" PropertyName="NSwagOutput" />
      </Exec>
      <!-- ... -->
    </Target>
    ```

After these changes, running `dotnet build` successfully completed without any NSwag-related errors, confirming that the runtime configurations were correctly aligned.
