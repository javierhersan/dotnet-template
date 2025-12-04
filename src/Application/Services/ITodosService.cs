using Domain.Entities;

namespace Application.Services;

public interface ITodosService
{
    Todo[] GetTodos();
    Todo? GetTodoById(int id);
}