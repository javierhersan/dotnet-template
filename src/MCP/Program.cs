using Application.Services;
using Microsoft.AspNetCore;
using static Microsoft.AspNetCore.Http.StatusCodes;
using API.Configuration;

/*
Using directive is unnecessary.IDE0005
namespace Microsoft.AspNetCore
*/

// var builder = WebApplication.CreateSlimBuilder(args);
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureSettings(builder.Configuration)
    .AddInfrastructure()
    .AddApplication()
    .ConfigureCors()
    .ConfigureOpenApi();

// builder.Services.AddControllers();

builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapOpenApi();
}

app.UseCors();
// app.UseAuthorization();
// app.MapControllers();
app.MapMcp("/mcp");
app.UseHttpsRedirection();

app.Run();
