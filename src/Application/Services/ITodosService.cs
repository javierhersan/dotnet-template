using Domain.Entities;

namespace Application.Services;

public interface ITodosService
{
    Result<Todo[]> GetTodos();
    Result<Todo?> GetTodoById(int id);
}