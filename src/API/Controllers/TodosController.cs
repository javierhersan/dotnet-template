using Microsoft.AspNetCore.Mvc;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly Todo[] todos = [];
    public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

    public TodosController()
    {
        todos = [
            new(1, "Walk the dog"),
            new(2, "Do the dishes", DateOnly.FromDateTime(DateTime.Now)),
            new(3, "Do the laundry", DateOnly.FromDateTime(DateTime.Now.AddDays(1))),
            new(4, "Clean the bathroom"),
            new(5, "Clean the car", DateOnly.FromDateTime(DateTime.Now.AddDays(2)))
        ];
    }

    [HttpGet]
    // [Route(nameof(GetTodos))]
    public ActionResult<Todo[]> GetTodos()
    {
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public ActionResult<Todo> GetTodoById(int id)
    {
        var todo = todos.FirstOrDefault(a => a.Id == id);
        return todo is not null ? Ok(todo) : NotFound();
    }
}