using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace MeuTodo.Controller
{
    [ApiController]
    [Route(template: "v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route(template: "todos")]
        public async Task<IActionResult> GetAsync(
            [FromServices] AppDbContext context)
        {
            var todos = await context //AppDbContext
                .Todos //DbSet<Todo>
                .AsNoTracking() //IQueryable<Todo>
                .ToListAsync(); //Task<List<...>>

            return Ok(todos);
        }

        [HttpGet]
        [Route(template: "todos/{id}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)

        {
            var todo = await context //AppDbContext
                .Todos //DbSet<Todo>
                .AsNoTracking() //IQueryable<Todo>
                .FirstOrDefaultAsync(x => x.Id == id); //Task<Todo>

            return todo == null ? NotFound() : Ok(todo);
        }

        [HttpPost(template: "todos")]
        public async Task<IActionResult> PostAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created(uri: $"v1/todos/{todo.Id}", todo);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpPut(template: "todos/{id}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model,
            [FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var todo = await context
                .Todos
                .FirstOrDefaultAsync(x => x.Id == id); //Task<Todo>

            if (todo == null)
                return NotFound();


            try
            {
                todo.Title = model.Title;

                context.Todos.Update(todo);
                await context.SaveChangesAsync();
                return Ok(todo);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [HttpDelete(template: "todos/{id}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] AppDbContext context,
            [FromRoute] int id)
        {

            var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id); //Task<Todo>

            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                return Ok(todo);

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}