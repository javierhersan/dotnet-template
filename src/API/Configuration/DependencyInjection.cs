using Application.Repositories;
using Application.Services;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace API.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ITodosRepository, TodosRepository>();
        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<ITodosService, TodosService>();
        return services;
    }
}