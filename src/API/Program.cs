using Application.Services;
using Microsoft.AspNetCore;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Scalar.AspNetCore;
using API.Configuration;

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
    .ConfigureAuthentication(builder.Configuration)
    .AddInfrastructure()
    .AddApplication()
    .ConfigureCors()
    .ConfigureOpenApi();

builder.Services.AddControllers();


var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
