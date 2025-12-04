using Domain.Entities;

namespace Application.Services;

public class TodosService : ITodosService
{
    private readonly Todo[] todos;

    public TodosService()
    {
        todos = [
            new Todo { Id = 1, Title = "Walk the dog" },
            new Todo { Id = 2, Title = "Do the dishes", DueBy = DateOnly.FromDateTime(DateTime.Now) },
            new Todo { Id = 3, Title = "Do the laundry", DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(1)) },
            new Todo { Id = 4, Title = "Clean the bathroom" },
            new Todo { Id = 5, Title = "Clean the car", DueBy = DateOnly.FromDateTime(DateTime.Now.AddDays(2)) }
        ];
    }

    public Todo[] GetTodos() => todos;

    public Todo? GetTodoById(int id) => todos.FirstOrDefault(a => a.Id == id);
}