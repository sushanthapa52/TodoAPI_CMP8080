using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoModels;

namespace TodoList.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDoController : ControllerBase
    {
        private readonly ApiDbContext _dbContext;

        public ToDoController(ApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetToDoItemsWithoutCompletedDate()
        {
            var todoItems = await _dbContext.ToDoItems
                .Where(item => item.CompletedDate == null)
                .ToListAsync();

            return Ok(todoItems);
        }
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateToDoItem([FromBody] TodoItem newToDoItem)
        {
            if (newToDoItem == null)
            {
                return BadRequest("Invalid payload");
            }

            _dbContext.ToDoItems.Add(newToDoItem);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetToDoItemById), new { id = newToDoItem.Id }, newToDoItem);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetToDoItemById(int id)
        {
            var todoItem = await _dbContext.ToDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return Ok(todoItem);
        }

        //Create a Post request that takes one argument of Id and fills in the CompletedDate with the current datetime.
        [HttpPost("{id}/complete")]
        public async Task<ActionResult> MarkToDoItemAsCompleted(int id)
        {
            var todoItem = await _dbContext.ToDoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            todoItem.CompletedDate = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle exceptions appropriately
                return StatusCode(500, "An error occurred while saving the data.");
            }

            return NoContent(); // 204 No Content
        }
    }
}
