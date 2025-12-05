using Application.Services;
using Microsoft.AspNetCore;
using static Microsoft.AspNetCore.Http.StatusCodes;
using API.Configuration;
using ModelContextProtocol.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

/*
Using directive is unnecessary.IDE0005
namespace Microsoft.AspNetCore
*/

// var builder = WebApplication.CreateSlimBuilder(args);
var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

builder.Configuration.AddConfiguration(configuration);

builder.Services
    .ConfigureSettings(builder.Configuration)
    .AddInfrastructure()
    .AddApplication()
    .ConfigureCors()
    .ConfigureOpenApi();

ApplicationSettings applicationSettings = builder.Configuration.GetSection("ApplicationSettings").Get<ApplicationSettings>() ?? new ApplicationSettings();

// MCP Authentication and Authorization
AzureAd azureAdSettings = applicationSettings.AzureAd;
string oAuthServerUrl = $"{azureAdSettings.Instance}{azureAdSettings.TenantId}/v2.0";
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = McpAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Authority = $"{azureAdSettings.Instance}{azureAdSettings.TenantId}/v2.0";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true, // False for multi audience scenarios
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = azureAdSettings.Audience,
        // ValidIssuer = $"{azureAdSettings.Instance}{azureAdSettings.TenantId}/v2.0",
        ValidIssuers = new[]
        {
            $"{azureAdSettings.Instance}{azureAdSettings.TenantId}/v2.0",
            $"https://sts.windows.net/{azureAdSettings.TenantId}/"
        },
        NameClaimType = "name",
        RoleClaimType = "roles"
    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var name = context.Principal?.Identity?.Name ?? "unknown";
            var email = context.Principal?.FindFirstValue("preferred_username") ?? "unknown";
            Console.WriteLine($"Token validated for: {name} ({email})");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"Challenging client to authenticate with Entra ID");
            return Task.CompletedTask;
        }
    };
})
.AddMcp(options =>
{
    options.ResourceMetadata = new()
    {
        Resource = new Uri(applicationSettings.McpServerUrl),
        ResourceDocumentation = new Uri($"{applicationSettings.McpServerUrl}/mcp"),
        AuthorizationServers = { new Uri(oAuthServerUrl) },
        ScopesSupported = ["mcp:tools"],
    };
});

// "Authorization": "Bearer eyJ0..."

builder.Services.AddAuthorization();
builder.Services.AddHttpClient(); 
builder.Services.AddHttpContextAccessor(); 

// builder.Services.AddControllers();

// https://den.dev/blog/mcp-csharp-sdk-authorization/
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();


/* -----------------------------------------------------------------------------------------------
    - Protects all MCP endpoints with a valid JWT token
    - Validate JWT user claims inside the MCP controller using the httpContextAccessor:

        ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
        string? userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
----------------------------------------------------------------------------------------------- */
app.MapMcp("/mcp").RequireAuthorization(); 

/* -----------------------------------------------------------------------------------------------
    - Exposes MCP endpoints without requiring authentication
    - Validates JWT inside the MCP controller using the httpContextAccessor:

        ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();
        if (!User.Identity?.IsAuthenticated ?? true)
        {
            // JWT validation handled by ASP.NET Core middleware when using app.UseAuthentication()
            // If the request contains a valid JWT (matching your config), the middleware sets User.Identity.IsAuthenticated = true.
            // If the token is invalid, IsAuthenticated will be false.
            throw new UnauthorizedAccessException("Authentication required.");
        }
----------------------------------------------------------------------------------------------- */
// app.MapMcp("/mcp");  

app.UseHttpsRedirection();

app.Run();
