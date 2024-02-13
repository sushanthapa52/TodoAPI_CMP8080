using Microsoft.EntityFrameworkCore;
using TodoModels;

namespace TodoList
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
        {
        }

        public DbSet<TodoItem> ToDoItems { get; set; }
    }
}
