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

    public Result<Todo[]> GetTodos()  
    {
        return new Result<Todo[]>()
        {
            Success = true,
            Data = _todosRepository.GetTodos()
        };
    }
        

    public Result<Todo?> GetTodoById(int id){
        return new Result<Todo?>()
        {
            Success = true,
            Data = _todosRepository.GetTodoById(id)
        };
    } 
}