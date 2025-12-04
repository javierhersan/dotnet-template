using Application.Services;
using Microsoft.AspNetCore;
using static Microsoft.AspNetCore.Http.StatusCodes;
using Infrastructure;

/*
Using directive is unnecessary.IDE0005
namespace Microsoft.AspNetCore
*/

// var builder = WebApplication.CreateSlimBuilder(args);
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApplicationSettings>(builder.Configuration.GetSection("MyConfig"));

builder.Services.AddInfrastructure();
builder.Services.AddSingleton<ITodosService, TodosService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
