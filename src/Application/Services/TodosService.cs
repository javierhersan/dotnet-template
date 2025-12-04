using Domain.Entities;
using Application.Repositories;

namespace Application.Services;

public class TodosService : ITodosService
{
    private readonly ITodosRepository _todosRepository;

    public TodosService(ITodosRepository todosRepository)
    {
        _todosRepository = todosRepository;
    }

    public Todo[] GetTodos() => _todosRepository.GetTodos();

    public Todo? GetTodoById(int id) => _todosRepository.GetTodoById(id);
}