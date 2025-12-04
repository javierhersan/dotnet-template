using System.ComponentModel;
using Application.Services;
using Domain.Entities;
using ModelContextProtocol.Server;
namespace API.Controllers;

[McpServerToolType]
public class TodosMcpController
{
    private readonly ITodosService _todosService;

    public TodosMcpController(ITodosService todosService)
    {
        _todosService = todosService;
    }

    [McpServerTool(Name = "GetTodos"), Description("Returns the list of todos.")]
    public Todo[] GetTodos()
    {
        return _todosService.GetTodos();
    }

    [McpServerTool(Name = "GetTodoById"), Description("Returns a todo by its ID.")]
    public Todo? GetTodoById(int id)
    {
        Todo? todo = _todosService.GetTodoById(id);
        return todo;
    }
}