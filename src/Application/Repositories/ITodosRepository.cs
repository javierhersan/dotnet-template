using Domain.Entities;

namespace Application.Repositories;

public interface ITodosRepository
{
    Todo[] GetTodos();
    Todo? GetTodoById(int id);
}