using Application.Repositories;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Application.Configuration;
using API.Controllers;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using AuthenticationService = Application.Services.AuthenticationService;

namespace API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApplicationSettings>(configuration.GetSection("ApplicationSettings"));
        services.Configure<AuthenticationSettings>(configuration.GetSection("AuthenticationSettings"));
        
        ApplicationSettings applicationSettings = configuration
            .GetSection("ApplicationSettings")
            .Get<ApplicationSettings>() ?? new ApplicationSettings();

        AuthenticationSettings authenticationSettings = configuration
            .GetSection("AuthenticationSettings")
            .Get<AuthenticationSettings>() ?? new AuthenticationSettings();

        services.AddSingleton(applicationSettings);
        services.AddSingleton(authenticationSettings);

        return services;
    }

    public static AuthenticationBuilder ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration, AuthenticationOptions? authenticationOptions = null)
    {
        AuthenticationSettings authenticationSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>() ?? new AuthenticationSettings();
        
        switch (authenticationSettings.AuthenticationType)
        {
            case AuthenticationType.JwtBearer:
                // self-issued JWT
                return services.ConfigureJwtBearerAuthentication(configuration, authenticationOptions);

            case AuthenticationType.MsEntraId:
                return services.ConfigureMsEntraIdAuthentication(configuration, authenticationOptions);

            case AuthenticationType.AzureAd:
                return services.ConfigureAzureAdAuthentication(configuration, authenticationOptions);

            case AuthenticationType.None:
            default:
                return services.ConfigureNoneAuthentication();
        } 
    }

    public static AuthenticationBuilder ConfigureMsEntraIdAuthentication(this IServiceCollection services, IConfiguration configuration, AuthenticationOptions? authOptions)
    {
        AuthenticationSettings authenticationSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>() ?? new AuthenticationSettings();

        AzureAd azureAdSettings = authenticationSettings.AzureAd;
        string oAuthServerUrl = $"{azureAdSettings.Instance}{azureAdSettings.TenantId}/v2.0";
        return services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = authOptions?.DefaultChallengeScheme ?? JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = authOptions?.DefaultAuthenticateScheme ?? JwtBearerDefaults.AuthenticationScheme;
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
        });
    }

    public static AuthenticationBuilder ConfigureJwtBearerAuthentication(this IServiceCollection services, IConfiguration configuration, AuthenticationOptions? authOptions)
    {
        AuthenticationSettings authenticationSettings = configuration.GetSection("AuthenticationSettings").Get<AuthenticationSettings>() ?? new AuthenticationSettings();

        // MCP Authentication and Authorization
        JwtBearer jwtBearerSettings = authenticationSettings.JwtBearer;
        return services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = authOptions?.DefaultChallengeScheme ?? JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = authOptions?.DefaultAuthenticateScheme ?? JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jwtBearerSettings.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtBearerSettings.IssuerKey)),
                ValidateAudience = true, // for multi-audience scenarios: false
                ValidAudience = jwtBearerSettings.Audience,
                ValidateLifetime = true,
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
                    Console.WriteLine($"Challenging client to authenticate with JWT Bearer");
                    return Task.CompletedTask;
                }
            };
        });
    }
    
    public static AuthenticationBuilder ConfigureAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration, AuthenticationOptions? authOptions)
    {
        AuthenticationBuilder authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultChallengeScheme = authOptions?.DefaultChallengeScheme ?? JwtBearerDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = authOptions?.DefaultAuthenticateScheme ?? JwtBearerDefaults.AuthenticationScheme;
        });

        authBuilder.AddMicrosoftIdentityWebApi(configuration.GetSection("AuthenticationSettings:AzureAd"));
        return authBuilder;
    }

    public static AuthenticationBuilder ConfigureNoneAuthentication(this IServiceCollection services)
    {
        return services.AddAuthentication("None")
            .AddScheme<AuthenticationSchemeOptions, AnonymousAuthenticationHandler>("None", options => { });
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IJwtAuthRepository, JwtAuthRepository>();
        services.AddSingleton<IOAuthRepository, OAuthRepository>();
        services.AddSingleton<IAuthenticationRepository, AuthenticationRepository>();
        services.AddSingleton<ITodosRepository, TodosRepository>();

        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ITodosService, TodosService>();
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }  

    public static IServiceCollection ConfigureOpenApi(this IServiceCollection services)
    {
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        return services;
    }
}

public class AnonymousAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public AnonymousAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity("None");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "None");
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}