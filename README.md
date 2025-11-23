# dotnet-template

## Prerequisites:

- .NET 10 SDK installed.

## Nomenclature

.NET 10 SDK includes:

- C# programming language and its developer tools.
- Roslyn compiler: compiles C# to IL (Intermediate Language) using AoT (ahead- of-time) compilation, or C# to native code (arm, x86, x86-64, etc) using AoT native compilation.
- CLR interpreter: interprets IL, and produces JIT compilation during executions.
- ASP.NET Core Framework for Web APIs

## Quick commands

1. Create a solution

```bash
dotnet new sln -n SolutionName
```

2. Create a Console project

```bash
dotnet new console -o src/ConsoleApp -n ConsoleApp
```

3. Create a Class library project

```bash
dotnet new classlib -o src/ClassLib -n ClassLib
```

4. Create an API project

```bash
# ASP.NET Core Web API template
dotnet new webapi -o src/WebApi -n WebApi
```

```bash
# ASP.NET Core Web API (Native AOT) template
dotnet new webapiaot -o src/WebApi -n WebApi
```

5. Add project to solution

```bash
dotnet sln SolutionName.sln add src/ConsoleApp/ConsoleApp.csproj
```

6. Add multiple projects to solution

```bash
dotnet sln SolutionName.sln add src/\*_/_.csproj
```

7. List solution projects

```bash
dotnet sln list
```

8. Remove project from solution

```bash
dotnet sln remove <path.csproj>
```

9. Restore packages

```bash
dotnet restore
```

\* Restores NuGet packages and project-to-project references required by the project or solution.

10. Build all projects within the solution

```bash
dotnet build
```

11. Run project

```bash
dotnet run --project src/ConsoleApp/ConsoleApp.csproj
```

12. Publish project

Common options:

- -c Release : release configuration.
- -o <folder> : publish output directory.
- -r <RID> : target runtime identifier for self-contained deployments (e.g. win-x64, linux-x64, osx.11.0-x64).
- --self-contained true|false : include the .NET runtime (self-contained) or rely on an installed runtime (framework-dependent).
- -p:PublishSingleFile=true : bundle into a single executable.
- -p:PublishTrimmed=true : trim unused assemblies (use cautiously; test thoroughly).

Publish commmand produces a deployable output (DLLs files or a self-contained executable) with build artifacts and dependencies ready for deployment.

There are several ways of compiling and publishing your project:

12.1. C# AoT compilation to IL without including the .NET runtime (CLR)

```bash
dotnet publish --project src/WebApi/WebApi.csproj -c Release -o ./publish
```

12.2. C# AoT compilation to IL including the .NET runtime (CLR)

```bash
dotnet publish --project src/WebApi/WebApi.csproj -c Release -r win-x64 --self-contained true -o ./publish/win-x64
```

12.3. C# AoT bundled compilation to IL including the .NET runtime (CLR)

```bash
dotnet publish --project src/WebApi/WebApi.csproj -c Release -r linux-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -o ./publish/linux-x64
```

12.4. C# AoT Native compilation

Add to .csproj file the following property

```xml
<PropertyGroup>
    <PublishAot>true</PublishAot>
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles> # Generates source files to map C# with Native Code
</PropertyGroup>
```

- There are some ASP.NET Core features not supported yet.

- Features that aren't compatible with Native AOT are disabled and throw exceptions at run time. A source analyzer is enabled to highlight code that isn't compatible with Native AOT. At publish time, the entire app, including NuGet packages, are analyzed for compatibility again.

- The dotnet publish command also compiles the source files and generates files that are compiled. In addition, dotnet publish passes the generated assemblies to a native IL compiler. The IL compiler produces the native executable. The native executable contains the native machine code.

- Many of the popular libraries used in ASP.NET Core projects currently have some compatibility issues when used in a project targeting Native AOT, such as:

- Use of reflection to inspect and discover types.
- Conditionally loading libraries at runtime.
- Generating code on the fly to implement functionality.
- Libraries using these dynamic features need to be updated in order to work with Native AOT. They can be updated using tools like Roslyn source generators. Library authors hoping to support Native AOT are encouraged to: Read about Native AOT compatibility requirements and prepare the library for trimming.
