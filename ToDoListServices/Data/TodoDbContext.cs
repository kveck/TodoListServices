namespace ToDoListServices.Data
{
    using Microsoft.EntityFrameworkCore;
    using ToDoListServices.Data.Models;

    public class TodoDbContext : DbContext
    {
        public DbSet<ToDoItem> ToDoItems { get; set; }
        public DbSet<TodoItemStatus> TodoItemStatuses { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// used for testing
        /// </summary>
        /// <param name="options"> tells the context all of its settings, such as which database to connect to</param>
        public TodoDbContext(DbContextOptions<TodoDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ToDoItem>().ToTable("ToDoItems");
            modelBuilder.Entity<TodoItemStatus>().ToTable("TodoStatus");

            base.OnModelCreating(modelBuilder);
        }
    }
}
