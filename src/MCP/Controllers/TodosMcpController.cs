using System.ComponentModel;
using ModelContextProtocol.Server;
namespace API.Controllers;

[McpServerToolType]
public class TodosMcpController
{
    private readonly Todo[] todos = [];
    public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    public TodosMcpController()
    {
        todos = [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        ];
    }

    [McpServerTool(Name = "GetTodos"), Description("Returns the list of todos.")]
    public Todo[] GetTodos()
    {
        return todos;
    }

    [McpServerTool(Name = "GetTodoById"), Description("Returns a todo by its ID.")]
    public Todo? GetTodoById(int id)
    {
        var todo = todos.FirstOrDefault(a => a.Id == id);
        return todo;
    }
}