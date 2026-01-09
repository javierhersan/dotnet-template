using System.ComponentModel;
using System.Security.Claims;
using Application.Services;
using Domain.Entities;
using ModelContextProtocol.Server;
namespace API.Controllers;

[McpServerToolType]
public class TodosMcpController
{
    private readonly ITodosService _todosService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TodosMcpController(ITodosService todosService, IHttpContextAccessor httpContextAccessor)
    {
        _todosService = todosService;
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User ?? new ClaimsPrincipal();

    [McpServerTool(Name = "GetTodos"), Description("Returns the list of todos.")]
    public Todo[]? GetTodos()
    {
        return _todosService.GetTodos().Data;
    }

    [McpServerTool(Name = "GetTodoById"), Description("Returns a todo by its ID.")]
    public Todo? GetTodoById(int id)
    {
        Todo? todo = _todosService.GetTodoById(id).Data;
        return todo;
    }

    [McpServerTool(Name = "GetProtectedTodoById"), Description("Returns a protected todo by its ID.")]
    public Todo? GetProtectedTodoById(int id)
    {
        // Not needed when used with app.MapMcp("/mcp").RequireAuthorization();
        // if (!User.Identity?.IsAuthenticated ?? true)
        // {
        //     // JWT validation handled by ASP.NET Core middleware when using app.UseAuthentication()
        //     // If the request contains a valid JWT (matching your config), the middleware sets User.Identity.IsAuthenticated = true.
        //     // If the token is invalid, IsAuthenticated will be false.
        //     throw new UnauthorizedAccessException("Authentication required.");
        // }

        // Use userEmail for authorization checks here.
        string? userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
        // if (authService.isAdmin(userEmail) == false)
        // {
        //     throw new UnauthorizedAccessException("User email claim not found.");
        // }

        Todo? todo = _todosService.GetTodoById(id).Data;
        return todo;
    }
}