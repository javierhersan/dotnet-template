using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Domain.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodosController : ControllerBase
    {
        private readonly ITodosService _todosService;

        public TodosController(ITodosService todosService)
        {
            _todosService = todosService;
        }

        [HttpGet]
        public ActionResult<Todo[]> GetTodos()
        {
            return Ok(_todosService.GetTodos());
        }

        [HttpGet("{id}")]
        public ActionResult<Todo> GetTodoById(int id)
        {
            var todo = _todosService.GetTodoById(id);
            return todo is not null ? Ok(todo) : NotFound();
        }

        /// <summary>
        /// Requires authentication.
        /// "Authorization": Bearer {token}
        /// </summary>
        [HttpGet("protected/{id}")]
        [Authorize]
        public ActionResult<Todo> GetProtectedTodos(int id)
        {
            string? userEmail = User.FindFirst(ClaimTypes.Name)?.Value;

            var todo = _todosService.GetTodoById(id);
            return todo is not null ? Ok(todo) : NotFound();
        }
    }
}